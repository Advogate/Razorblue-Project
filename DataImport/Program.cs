using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class ImportData
{
    // The name of the primary key column in the CSV file
    private const string PrimaryKeyColumn = "Car Registration";

    public static void Main()
    {
        ImportData importData = new ImportData();
        // Read the data from the CSV file
        List<Dictionary<string, string>> rows = ReadCsv("TechnicalTestData.csv");

        // Remove any duplicates by using the primary key
        rows = RemoveDuplicates(rows);

        // Group the rows by fuel type
        var rowsByFuelType = rows.GroupBy(row => row["Fuel"]);

        // Create a CSV file for each fuel type
        foreach (var fuelTypeGroup in rowsByFuelType)
        {
            // Get the fuel type
            string fuelType = fuelTypeGroup.Key;

            // Get the rows for the fuel type
            List<Dictionary<string, string>> fuelTypeRows = fuelTypeGroup.ToList();

            //Create the CSV file
           importData.CreateCsvFile(fuelType + ".csv", fuelTypeRows);
        }

        //list of vehicles that match the following criteria for a valid, current style registration
        IEnumerable<IGrouping<string, Dictionary<string,string>>> rowsByRegistration = rows.GroupBy(row => row["Car Registration"]);
        importData.GetMatchingStrings((rowsByRegistration.SelectMany(cr=>cr.Select(d=>d["Car Registration"])).ToList()));
        Console.Read();
    }
    public void GetMatchingStrings(List<string> strings)
    {
        // The regular expression pattern to match
        string pattern = @"^[A-Z]{2}\d{2} [A-Z]{3}$";

        // Compile the regular expression
        Regex regex = new Regex(pattern);

        // Create a list to store the matching strings
        List<string> matchingStrings = new List<string>();

        // Iterate through the strings
        foreach (string s in strings)
        {
            // Check if the string matches the regular expression
            if (regex.IsMatch(s))
            {
                // If the string matches the regular expression, add it to the list
                Console.WriteLine(s);
            }
        }

    }
    private void CreateCsvFile(string fileName, List<Dictionary<string, string>> rows)
    {
        // Create a new file
        using (StreamWriter file = new StreamWriter(fileName))
        {
            // Get the keys (column names) of the first row
            string[] keys = rows[0].Keys.ToArray();

            // Write the column names to the file
            file.WriteLine(string.Join(",", keys));

            // Iterate through the rows
            foreach (Dictionary<string, string> row in rows)
            {
                // Write the values to the file
                file.WriteLine(string.Join(",", keys.Select(key => row[key])));
            }
        }
    }

    // Reads the data from a CSV file and returns it as a list of dictionaries
    private static List<Dictionary<string, string>> ReadCsv(string fileName)
    {
        List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

        // Read the lines from the file
        string[] lines = File.ReadAllLines(fileName);

        // Split the first line (the header) into column names
        string[] header = lines[0].Split(",");

        // Iterate through the rest of the lines (the data)
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(",");
            Dictionary<string, string> row = new Dictionary<string, string>();
            for (int j = 0; j < values.Length; j++)
            {
                row[header[j]] = values[j];
            }
            rows.Add(row);
        }

        return rows;
    }

    // Removes any duplicates from the list of rows by using the primary key
    private static List<Dictionary<string, string>> RemoveDuplicates(List<Dictionary<string, string>> rows)
    {
        // Create a list to store the unique rows
        List<Dictionary<string, string>> uniqueRows = new List<Dictionary<string, string>>();

        // Create a HashSet to store the primary keys of the rows that have been added to the list
        HashSet<string> primaryKeys = new HashSet<string>();

        // Iterate through the rows
        foreach (Dictionary<string, string> row in rows)
        {
            // Get the primary key of the current row
            string primaryKey = row[PrimaryKeyColumn];

            // If the primary key has not been seen before, add the row to the list and add the primary key to the HashSet
            if (!primaryKeys.Contains(primaryKey))
            {
                uniqueRows.Add(row);
                primaryKeys.Add(primaryKey);
            }
        }

        return uniqueRows;
    }
}