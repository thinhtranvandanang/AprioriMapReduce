using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordNET9
{
    internal class clsFile
    {

        public static void WriteDicListStringIntToFile(Dictionary<List<string>, int> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var result in results)
                {
                    // result.Key là List<string>, cần chuyển đổi thành chuỗi
                    string keyString = string.Join(",", result.Key); // Chuyển đổi danh sách thành chuỗi
                    writer.WriteLine($"{keyString}: {result.Value}"); // Ghi chuỗi vào tệp
                }
            }
        }

        public static void WriteListListStringToFile2(List<List<string>> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var result in results)
                {
                    // Convert the list of strings to a single string
                    string keyString = string.Join(",", result);
                    writer.WriteLine(keyString); // Write the string to the file
                }
            }
        }


        public static void WriteDictionaryStringIntToFile(Dictionary<string, int> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var kvp in results)
                {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        public static void WriteListListIntToFile(List<List<int>> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var list in results)
                {
                    writer.WriteLine(string.Join(",", list));
                }
            }
        }

        public static void WriteListListStringToFile(List<List<string>> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var list in results)
                {
                    writer.WriteLine(string.Join(",", list));
                }
            }
        }

        public static void WriteSalesToFile(List<Sale> sales, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var sale in sales)
                {
                    writer.WriteLine($"{sale.TransactionID},{sale.ProductID}");
                }
            }
        }

        public static void WriteIDictionaryToFile(Dictionary<int, int> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var kvp in results)
                {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }
        public static void WriteIDictionaryStringIntToFile(Dictionary<string, List<int>> groupedTransactions, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var entry in groupedTransactions)
                {
                    string transactionID = entry.Key;
                    List<int> productIDs = entry.Value;

                    writer.WriteLine($"{transactionID}: {string.Join(", ", productIDs)}");
                }
            }
        }

        public static void WriteDictionaryToFile(Dictionary<string, List<int>> groupedTransactions, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var entry in groupedTransactions)
                {
                    string transactionID = entry.Key;
                    List<int> productIDs = entry.Value;

                    writer.WriteLine($"{transactionID}: {string.Join(",", productIDs)}");
                }
            }
        }
        public static void WriteDatasetToFile(int[][] dataset, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var row in dataset)
                {
                    // Ghi ra tệp: các mã hàng hóa phân tách bằng dấu phẩy
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }

        public static void WriteListDicToFile(List<Dictionary<int, int>> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int partitionIndex = 1;

                foreach (var result in results)
                {
                    writer.WriteLine($"Partition {partitionIndex}:");
                    foreach (var kvp in result)
                    {
                        writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                    writer.WriteLine(); // Dòng trống giữa các phân đoạn
                    partitionIndex++;
                }
            }
        }


        public static void WriteListStringToFile(List<string> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var result in results)
                {
                    writer.WriteLine(result);
                }
            }
        }
        public static void WriteListDicStringIntToFile(List<Dictionary<string, int>> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var result in results)
                {
                    foreach (var kvp in result)
                    {
                        writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                }
            }
        }







    }
}
