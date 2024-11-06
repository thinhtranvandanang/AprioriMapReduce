using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordNET9
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.SqlClient;
    using System.Net;

    public class Program
    {
        static string connectionString = @"Data Source=.;Initial Catalog=CSDLBanHangWsmart;Integrated Security=True";
        static string tableFullData = "SalesMonth"; // 16267, 10%=16267, 20%=3255
        static string tableTestData = "Sales"; // 274, 10%=27, 20%=55
        static int minSupport = 2; // Đặt giá trị hỗ trợ tối thiểu
        static double minConfidence = 0.2; // Đặt ngưỡng độ tin cậy tối thiểu
        public static void Main()
        {
            // Bước 1: Đọc dữ liệu từ SQL Server và chuyển đổi sang định dạng Apriori
            List<Sale> sales = clsSQLServer.ReadFromSQLServerToList(connectionString, tableFullData);
            //clsFile.WriteSalesToFile(sales, "sales.txt");
            var dataset = clsSQLServer.ConvertToAprioriFormat(sales);
            //clsFile.WriteDatasetToFile(dataset, "dataset.txt");
            // Bước 2: Khởi tạo Apriori Algorithm và các biến Mapper và Reducer
            AprioriAlgorithm aprioriAlgorithm = new AprioriAlgorithm();
            AprioriReducer reducer = new AprioriReducer();
            // Bước 3: Thực hiện song song giai đoạn Map bằng cách chia dataset thành các phân đoạn
            int numPartitions = 4; // Số lượng phân đoạn để xử lý song song
            var itemCounts = aprioriAlgorithm.Map(dataset, numPartitions);
            //clsFile.WriteListDicStringIntToFile(itemCounts, "itemCounts1.txt");
            // Bước 4: Thực hiện Reduce để lọc ra các tập hợp item đạt ngưỡng minSupport
            var largeItemsets = reducer.Reduce(itemCounts, minSupport);
            //clsFile.WriteDictionaryStringIntToFile(largeItemsets, "largeItemsets1.txt");
            
            // Lặp lại cho các cấp độ tiếp theo nếu cần
            int level = 2;
            var allLargeItemsets = new Dictionary<string, int>(); // Lưu trữ tất cả Large Itemsets
            while (largeItemsets.Count > 0)
            {
                // Lưu trữ các large itemsets của cấp độ hiện tại
                foreach (var itemset in largeItemsets)
                {
                    allLargeItemsets[itemset.Key] = itemset.Value;
                }
                //clsFile.WriteDictionaryStringIntToFile(allLargeItemsets, "allLargeItemsets" + level + ".txt");
                
                // Tạo tập hợp ứng viên từ các large itemsets đã tìm được
                var currentItems = largeItemsets.Keys.ToList();
                //clsFile.WriteListStringToFile(currentItems, "currentItems" + level + ".txt");
                var candidates = aprioriAlgorithm.GenerateCandidates(currentItems, level); // tạo tập hợp candidates, mổi item có level phần tử 
                //clsFile.WriteListListStringToFile(candidates, "candidates" + level + ".txt");
                // Tạo lại dataset từ tập hợp ứng viên, chỉ chọn các dataset có chứa candidates
                var candidateDataset = aprioriAlgorithm.CreateCandidateDataset(dataset, candidates);
                //clsFile.WriteDatasetToFile(candidateDataset, "candidateDataset" + level + ".txt");
                // Áp dụng Map song song trên dataset mới candidates
                // Đếm các giao dịch, chứa đồng thời level item trong tập 
                itemCounts = aprioriAlgorithm.Map2(candidateDataset, numPartitions, candidates); // đếm  - tập hợp candidates có level phần tử (bởi dấu ,)
                //clsFile.WriteListDicStringIntToFile(itemCounts, "itemCounts" + level + ".txt");
                largeItemsets = reducer.Reduce(itemCounts, minSupport);// đã lọc ra các tập hợp có level phần tử >  minSupport
                clsFile.WriteDictionaryStringIntToFile(largeItemsets, "largeItemsets" + level + ".txt");
                level++;
            }
            // Bước 5: Sinh các luật kết hợp dựa trên độ tin cậy
            Console.WriteLine("Association Rules:");

            GenerateAssociationRules(allLargeItemsets, minConfidence);

            Console.WriteLine("Apriori Algorithm Finished.");

            Console.ReadKey();
        }
        public static void GenerateAssociationRules(Dictionary<string, int> largeItemsets, double minConfidence)
        {
            int dem = 0;
            foreach (var itemset in largeItemsets)
            {
                // Tách chuỗi itemset.Key thành danh sách các phần tử
                var items = itemset.Key.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                int supportAB = itemset.Value;
                dem++;  // chỉ phục vụ in file để xem thông tin 

                // Chỉ tạo luật cho các itemsets có nhiều hơn một phần tử
                if (items.Count > 1)
                {
                    // Lấy tất cả các tập con của itemset để tạo luật kết hợp
                    var subsets = GetSubsets(items);
                    // Duyệt qua từng tập con và tính độ tin cậy
                    foreach (var subset in subsets)
                    {
                        // Chuyển subset thành chuỗi để tìm trong largeItemsets
                        string subsetKey = string.Join(",", subset);
                        // Lấy support của tập con hiện tại (A)
                        if (largeItemsets.TryGetValue(subsetKey, out int supportA) && supportA > 0)
                        {
                            // Tính phần tử còn lại trong tập (B)
                            var remainingItems = items.Except(subset).ToList();
                            double confidence = (double)supportAB / supportA;

                            // Kiểm tra nếu độ tin cậy >= ngưỡng minConfidence thì in ra luật
                            if (confidence >= minConfidence && remainingItems.Count > 0)
                            {
                                // Chuyển đổi từng mã trong subset và remainingItems thành tên sản phẩm
                                var subsetNames = new List<string>();
                                foreach (var maHH in subset)
                                {
                                    subsetNames.Add(productNames(int.Parse(maHH)));
                                }

                                var remainingNames = new List<string>();
                                foreach (var maHH in remainingItems)
                                {
                                    remainingNames.Add(productNames(int.Parse(maHH)));
                                }

                                // In luật kết hợp với tên sản phẩm
                                Console.WriteLine($"Rule: {{ {string.Join(", ", subsetNames)} }} => {{ {string.Join(", ", remainingNames)} }}, " +
                                                  $"Confidence: {confidence:F2}");
                            }
                        }
                    }
                }
            }
        }

        public static void GenerateAssociationRules1(Dictionary<string, int> largeItemsets, double minConfidence)
        {
            int dem = 0;
            foreach (var itemset in largeItemsets)
            {
                // Tách chuỗi itemset.Key thành danh sách các phần tử
                var items = itemset.Key.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                int supportAB = itemset.Value;
                // In số lượng phần tử trong itemset
                // Console.WriteLine($"{dem} {itemset.Key} items.Count : {items.Count}");
                dem++;  // chỉ phục vụ in file
                // Chỉ tạo luật cho các itemsets có nhiều hơn một phần tử
                if (items.Count > 1)
                {
                    // Lấy tất cả các tập con của itemset để tạo luật kết hợp
                    var subsets = GetSubsets(items); 
                    // clsFile.WriteListListStringToFile2(subsets, $"subsets_{dem}.txt");
                    // Duyệt qua từng tập con và tính độ tin cậy
                    foreach (var subset in subsets)
                    {
                        // Chuyển subset thành chuỗi để tìm trong largeItemsets
                        string subsetKey = string.Join(",", subset);
                        // Lấy support của tập con hiện tại (A)
                        if (largeItemsets.TryGetValue(subsetKey, out int supportA) && supportA > 0)
                        {
                            // Tính phần tử còn lại trong tập (B)
                            var remainingItems = items.Except(subset).ToList();
                            double confidence = (double)supportAB / supportA;
                            // Kiểm tra nếu độ tin cậy >= ngưỡng minConfidence thì in ra luật
                            if (confidence >= minConfidence && remainingItems.Count > 0)
                            {
                                Console.WriteLine($"Rule: {{ {subsetKey} }} => {{ {string.Join(",", remainingItems)} }}, " +
                                                  $"Confidence: {confidence:F2}");
                            }
                        }
                    }
                }
            }
        }




        // Hàm lấy tất cả các tập con không rỗng của một danh sách phần tử
        private static List<List<string>> GetSubsets(List<string> itemset)
        {
            var subsets = new List<List<string>>();
            int subsetCount = (1 << itemset.Count) - 1; // 2^n - 1 tập con không rỗng

            for (int i = 1; i <= subsetCount; i++)
            {
                var subset = new List<string>();
                for (int j = 0; j < itemset.Count; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        subset.Add(itemset[j]);
                    }
                }
                subsets.Add(subset);
            }

            return subsets;
        }

        public static string productNames(int maHH)
        { // luật không nhiều, nên không cần đọc tất cả vào bộ nhớ.
          // truy xuất csdl để lấy tên hàng hóa khi cần
            string tenHangHoa = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TENHH FROM HangHoa WHERE ProductID = @MaHangHoa";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaHangHoa", maHH);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tenHangHoa = reader["TENHH"].ToString();
                        }
                    }
                }
            }
            return tenHangHoa;
        }


    }
}