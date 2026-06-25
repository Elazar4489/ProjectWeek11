using System;
using System.IO;
using System.Net.NetworkInformation;
namespace PWeek11
{
    enum ReportType 
    { 
        Collect, 
        Analyze, 
        Recon, 
        Intel 
    }
    enum ReportStatus
    {
        Pending,
        Approved,
        Rejected
    }
    class ReportFileManager
    {
        static void Main()
        {
            string[]? allLines = LoadFile("C:\\Users\\elazar\\C#Projects\\ProgectWeek11\\reports.txt");
            if (allLines == null)
            {
                return;
            }

            string[] unitNames = new string[100];
            ReportType?[] reportTypes = new ReportType?[100];
            int[] priorities = new int[100];
            double[] scores = new double[100];
            ReportStatus?[] statuses = new ReportStatus?[100];
           
            int validCount = ProcessReports(allLines, unitNames, reportTypes, priorities, scores, statuses);
            Console.WriteLine($"Stored {validCount} valid records for analysis");
            DisplayBasicStatistics(scores, validCount);
            DisplayStatusCounts(statuses);
            DisplayTypeCounts(reportTypes);
            DisplayHighestPriorityApproved(unitNames, reportTypes, priorities, scores, statuses, validCount);
            DisplayAverageByPriority(priorities, scores, validCount);

        }



        static string[]? LoadFile(string path)
        {
            if (File.Exists(path))
            {
                string[] AllLines = File.ReadAllLines(path);
                if (AllLines.Length == 0)
                {
                    Console.WriteLine("Error: File is empty.");
                    return null;
                }
                else
                {
                    Console.WriteLine($"File loaded: {AllLines.Length} lines found");
                    return AllLines;
                }
            }
            else
            {
                string fileName = Path.GetFileName(path);
                Console.WriteLine($"Error: File {fileName} not found.");
                return null;
            }
        }
        static int ProcessReports(string[] lines, string[] unitNames, ReportType?[] reportTypes, int[] priorities, double[] scores, ReportStatus?[] status)
        {
            int ValidLines = 0;
            int InvalidLines = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string[]? line = GetSplitLine(lines[i]);
                if (line != null && CheckData(line))
                {
                    ValidLines++;
                    ArrayStorage(line, unitNames, reportTypes, priorities, scores, status);
                }
                else
                {
                    InvalidLines++;
                }

            }
            Console.WriteLine($"Processing complete\r\nValid records: {ValidLines}\r\nInvalid records: {InvalidLines}");
            return ValidLines;
        }
        
        static double CalculateAverage(double[] scores, int validReports)
        {
            double sum = new double();
            double countR = (double)validReports;
            foreach (double score in scores)
            {
                sum += score;
            }
            if (validReports>0)
            { 
                return sum / (double)validReports; 
            }
            return 0.0;
        }
        static double FindMaxScore(double[] scores)
        {
            double max = scores[0];
            foreach (double score in scores)
            {
                if (score > max)
                {
                    max = score;
                }
            }
            return max;
        }
        static double FindMinScore(double[] scores)
        {
            double min = scores[0];
            foreach (double score in scores)
            {
                if (score < min)
                {
                    min = score;
                }
            }
            return min;
        }
        static int CountByStatus(ReportStatus?[] statuses, ReportStatus status)
        {
            int countStatus = 0;
            foreach (ReportStatus? Astatus in statuses)
            {
                if (Astatus == status)
                {
                    countStatus += 1;
                }
            }
            return countStatus;
        }
        static int CountByType(ReportType?[] reportTypes, ReportType theType)
        {
            int countType = 0;
            foreach (ReportType? Atype in reportTypes)
            {
                if (Atype == theType)
                {
                    countType += 1;
                }
            }
            return countType;
        }
        static void DisplayBasicStatistics(double[] scores, int validReports)
        {
            double Average = CalculateAverage(scores, validReports);
            double max = FindMaxScore(scores);
            double min = FindMinScore(scores);
            Console.WriteLine(
                $"=== Report Statistics ===\n\r" +
                $"Total Reports: {validReports}\n\r" +
                $"Average Score: {Average}\n\r" +
                $"Highest Score: {max}\n\r" +
                $"Lowest Score: {min}\n\r"
                );

        }
        static void DisplayStatusCounts(ReportStatus?[] statuses)
        {
            int pending = CountByStatus(statuses, ReportStatus.Pending);
            int approved = CountByStatus(statuses, ReportStatus.Approved);
            int rejected = CountByStatus(statuses, ReportStatus.Rejected);
            Console.WriteLine(
                $"=== Reports by Status ===\n\r" +
                $"Pending{pending}\n\r" +
                $"Approved{approved}\n\r" +
                $"Rejected{rejected}");

        }
        static void DisplayTypeCounts(ReportType?[] reportTypes)
        {
            int collect = CountByType(reportTypes, ReportType.Collect);
            int analyze = CountByType(reportTypes, ReportType.Analyze);
            int recon = CountByType(reportTypes, ReportType.Recon);
            int intel = CountByType(reportTypes, ReportType.Intel);
            Console.WriteLine(
                $"=== Reports by Type ===\n\r" +
                $"Collect: {collect}\n\r" +
                $"Analyze: {analyze}\n\r" +
                $"Recon: {recon}\n\r" +
                $"Intel: {intel}\n\r"
                );
        }
        static void DisplayHighestPriorityApproved(string[] names, ReportType?[] types, int[] priorities, double[] scores, ReportStatus?[] statuses, int validReports)
        {
            int highP = 0;
            int index = 0;
            for (int i = 0; i < validReports; i++)
            {
                if (statuses[i] == ReportStatus.Approved && priorities[i] < highP)
                {
                    highP = priorities[i];
                    index = i;
                }
            }
            Console.WriteLine(
                    $"=== Highest Priority Approved Report ===\n\r" +
                    $"Unit: {names[index]}\n\r" +
                    $"Type: {types[index]}\n\r" +
                    $"Priority: {priorities[index]}\n\r" +
                    $"Score: {scores[index]}\n\r");

            //int[] highestPriorityIndexes = GetIndexesListOfHighestPriority(statuses, priorities, GetHighestPriority(priorities));
            //foreach (int i in highestPriorityIndexes)
            //{
            //    Console.WriteLine(
            //        $"=== Highest Priority Approved Report ===\n\r" +
            //        $"Unit: {names[i]}\n\r" +
            //        $"Type: {types[i]}\n\r" +
            //        $"Priority: {priorities[i]}\n\r" +
            //        $"Score: {scores[i]}\n\r");
            //}
        }
        static void DisplayAverageByPriority(int[] priorities, double[] scores, int validReports)
        {
            string one = AverageByPriority(priorities, scores, validReports, 1);
            string two = AverageByPriority(priorities, scores, validReports, 2);
            string three = AverageByPriority(priorities, scores, validReports, 3);
            string four = AverageByPriority(priorities, scores, validReports, 4);
            string five = AverageByPriority(priorities, scores, validReports, 5);
            Console.WriteLine(
                $"=== Average Score by Priority ===\n\r" +
                $"Priority: 1 {one}\n\r" +
                $"Priority: 2 {two}\n\r" +
                $"Priority: 3 {three}\n\r" +
                $"Priority: 4 {four}\n\r" +
                $"Priority: 5 {five}"
                );
        }










        //=============================================================================================================
        // ============================== Utils =======================================


        static string[]? GetSplitLine(string line)
        {
            string[] splitLine = line.Split(",");
            if (splitLine.Length != 5)
            {
                return null;
            }
            return splitLine;
        }
        static bool CheckData(string[] lines)
        {
            if (lines[0].Length > 1)
            {
                if (Enum.TryParse(lines[1], true, out ReportType theType))
                {
                    if (int.TryParse(lines[2], out int priority) && priority > 0 && priority < 6)
                    {
                        if (double.TryParse(lines[3], out double score) && score > 0.0 && score <= 100.0)
                        {
                            if (Enum.TryParse(lines[4], true, out ReportStatus status))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        static string ArrayStorage(string[] line, string[] unitNames, ReportType?[] reportTypes, int[] priorities, double[] scores, ReportStatus?[] statuses)
        {
            string name = line[0];
            ReportType RType = Enum.Parse<ReportType>(line[1], true);
            int priority = int.Parse(line[2]);
            double score = double.Parse(line[3]);
            ReportStatus status = Enum.Parse<ReportStatus>(line[4], true);
            for (int i = 0; i < unitNames.Length; i++)
            {
                if (string.IsNullOrEmpty(unitNames[i]))
                {
                    unitNames[i] = name;
                    reportTypes[i] = RType;
                    priorities[i] = priority;
                    scores[i] = score;
                    statuses[i] = status;
                    break;
                }
            }
            return "done";
        }

        //static int GetHighestPriority(int[] priorities)
        //{
        //    int highest = 0;
        //    foreach (int priority in priorities)
        //    {
        //        if (priority > highest)
        //        {
        //            highest = priority;
        //        }
        //    }
        //    return highest;
        //}
        //static int[] GetIndexesListOfHighestPriority(ReportStatus?[] statuses, int[] priorities, int highestPriority)
        //{
        //    int[] highestPriorityIndexes = new int[100];
        //    int j = 0;
        //    for (int i = 0; i < statuses.Length; i++)
        //    {
        //        if (statuses[i] == ReportStatus.Approved && priorities[i] == highestPriority)
        //        {
        //            highestPriorityIndexes[j] = i;
        //            j += 1;
        //        }
        //    }
        //    return highestPriorityIndexes;
        //}
        
        //static int CountValidReports(ReportType?[] arrr)
        //{
        //    int validReports = 0;
        //    for (int i = 0; i < arrr.Length; i++)
        //    {
        //        if (arrr[i] is not null)
        //        {
        //            validReports++;
        //        }
        //    }
        //    return validReports;
        //}
        static string AverageByPriority(int[] priorities, double[] scores, int validReports, int numPrioriy)
        {
            double sumOfThisPriority = new double();
            double countOfThisPriority = new double();
            for (int i = 0; i < validReports; i++)
            {
                if (priorities[i] == numPrioriy)
                {
                    sumOfThisPriority += scores[i];
                    countOfThisPriority++;
                }
            }
            if (sumOfThisPriority > 0)
            {
                double average = sumOfThisPriority / countOfThisPriority;
                string averagestr = average.ToString("F2");
                return averagestr;
            }
            return "No reports";
        }
    }
}