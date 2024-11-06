using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordNET9
{
    public class AprioriAlgorithm
    {
        // Phương thức Map có chia phân đoạn và đếm từng phân đoạn
        // Đếm các giao dịch, có chứa 
        // Đếm các tập có số lượng lớn hơn 2 trong thuật toán apriori
        public List<Dictionary<string, int>> Map2(int[][] dataset, int numPartitions, List<List<string>> candidateItemsets)
        {
            var partitionedResults = new List<Dictionary<string, int>>();
            int partitionSize = (int)Math.Ceiling((double)dataset.Length / numPartitions);
            var partitions = PartitionDataset(dataset, partitionSize);

            // Áp dụng Map trên từng phân đoạn
            Parallel.ForEach(partitions, partition =>
            {
                var itemCounts = new Dictionary<string, int>();
                foreach (var transaction in partition)
                {
                    var transactionSet = new HashSet<int>(transaction);

                    // Duyệt qua từng ứng viên và kiểm tra xem nó có trong giao dịch không
                    foreach (var candidate in candidateItemsets)
                    {
                        if (candidate.All(item => transactionSet.Contains(int.Parse(item))))
                        {
                            string candidateKey = string.Join(",", candidate);

                            if (itemCounts.ContainsKey(candidateKey))
                                itemCounts[candidateKey]++;
                            else
                                itemCounts[candidateKey] = 1;
                        }
                    }
                }

                // Thêm kết quả đếm từ mỗi phân đoạn vào danh sách kết quả
                lock (partitionedResults)
                {
                    partitionedResults.Add(itemCounts);
                }
            });

            // Trả về kết quả đếm từ các phân đoạn
            return partitionedResults;
        }
        public List<Dictionary<string, int>> Map(int[][] dataset, int numPartitions)
        {
            var partitionedResults = new List<Dictionary<string, int>>();
            // Tạo phân đoạn từ dataset
            int partitionSize = (int)Math.Ceiling((double)dataset.Length / numPartitions);
            var partitions = PartitionDataset(dataset, partitionSize);
            // Áp dụng Map trên từng phân đoạn
            Parallel.ForEach(partitions, partition =>
            {
                var itemCounts = new Dictionary<string, int>();
                foreach (var transaction in partition)
                {
                    var uniqueItems = transaction.Distinct();
                    foreach (var item in uniqueItems)
                    {
                        string itemKey = item.ToString(); // Chuyển đổi item sang kiểu string
                        if (itemCounts.ContainsKey(itemKey))
                            itemCounts[itemKey]++;
                        else
                            itemCounts[itemKey] = 1;
                    }
                }
                lock (partitionedResults)
                {
                    partitionedResults.Add(itemCounts);
                }
            });

            // Hợp nhất kết quả từ các phân đoạn, sẽ thực hiện Reduce trong class - AprioriReducer 
            return partitionedResults;
        }
       
        public int[][] CreateCandidateDataset(int[][] dataset, List<List<string>> candidateItemsets)
        {
            var candidateDataset = new List<int[]>();
            foreach (var transaction in dataset)
            {
                var transactionSet = new HashSet<int>(transaction);

                foreach (var candidate in candidateItemsets)
                {
                    // Chuyển đổi từng item của candidate sang int để so sánh
                    if (candidate.All(item => int.TryParse(item, out int itemInt) && transactionSet.Contains(itemInt)))
                    {
                        candidateDataset.Add(transaction);
                        break;
                    }
                }
            }
            return candidateDataset.ToArray();
        }

        // Hàm đệ quy để tạo ra tất cả các tổ hợp có `n` phần tử
        private List<int[][]> PartitionDataset(int[][] dataset, int partitionSize)
        {
            var partitions = new List<int[][]>();
            for (int i = 0; i < dataset.Length; i += partitionSize)
            {
                partitions.Add(dataset.Skip(i).Take(partitionSize).ToArray());
            }
            return partitions;
        }

        public List<List<string>> GenerateCandidates(List<string> currentItemsets, int n)
        {
            var candidates = new List<List<string>>();

            // Phân tách các phần tử từ các dòng trong currentItemsets
            var items = new HashSet<string>(); // Sử dụng HashSet để loại bỏ trùng lặp
            foreach (var line in currentItemsets)
            {
                foreach (var item in line.Split(',').Select(item => item.Trim()))
                {
                    items.Add(item); // Thêm vào HashSet sẽ tự động loại bỏ trùng lặp
                }
            }

            // Chuyển đổi HashSet trở lại thành danh sách
            var uniqueItems = items.ToList();

            // Kiểm tra nếu số phần tử ít hơn `n`, không tạo tổ hợp
            if (uniqueItems.Count < n)
            {
                return candidates;
            }

            // Gọi hàm để tạo ra các tổ hợp có n phần tử
            GenerateCombinations(uniqueItems, n, candidates);
            return candidates;
        }

        private void GenerateCombinations(List<string> items, int n, List<List<string>> candidates)
        {
            int[] indices = new int[n];

            // Khởi tạo chỉ số ban đầu [0, 1, ..., n-1]
            for (int i = 0; i < n; i++)
            {
                indices[i] = i;
            }

            while (indices[0] <= items.Count - n)
            {
                // Tạo tổ hợp từ các chỉ số hiện tại
                var candidate = new List<string>();
                for (int i = 0; i < n; i++)
                {
                    candidate.Add(items[indices[i]]);
                }
                candidates.Add(candidate);

                // Tìm vị trí cuối cùng có thể tăng chỉ số
                int pos = n - 1;
                while (pos >= 0 && indices[pos] == items.Count - n + pos)
                {
                    pos--;
                }

                // Nếu không còn vị trí nào có thể tăng, dừng vòng lặp
                if (pos < 0)
                {
                    break;
                }

                // Tăng chỉ số tại vị trí `pos` và thiết lập lại các chỉ số sau nó
                indices[pos]++;
                for (int i = pos + 1; i < n; i++)
                {
                    indices[i] = indices[i - 1] + 1;
                }
            }
        }








    }
}



