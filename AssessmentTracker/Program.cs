/*
 * Project: Asssesment Tracker
 * 
 * Version: 0.1.0
 * 
 * Description:  a simple console application that stores the details of any 
assessments (unit or cluster name, assessment number or name, the type of 
assessment and the due date)
 *
 * Contains: Main and 5 other Methods
 * 
 * Written by: John Richardson (M196909)
 * 
 * Date: 20/08/2026
 */

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Globalization;
namespace AssessmentTracker
{
    internal class Program
    {
        /*
         * Main Method
         * Input Requirements: user to enter numbers to select menu items, Unitinfo.txt
         * Output Expections: initiate methods from user input
         */
        static void Main(string[] args)
        {
            Console.WriteLine("Assessment Tracker");
            string textFile = "Unitinfo.txt";
            //Load existing from the file using the separate method.
            List<string> assessment = LoadAssessmentsFromFile(textFile);

            //To enable us to exit the loop
            bool exit = false;

            //While exit == false
            while (!exit)
            {
                //Display the menu options
                Console.WriteLine("==== Assessment Tracker ====");
                Console.WriteLine("\nMenu");
                Console.WriteLine("1. View Assessment");
                Console.WriteLine("2. Add Assessment");
                Console.WriteLine("3. Delete Assessment");
                Console.WriteLine("4. Exit");
                Console.WriteLine("Select an option: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    //if the user selects option 1
                    case "1":
                        DisplayAssessment(assessment);
                        break;
                    //if the user selects option 2
                    case "2":
                        AddAssessment(assessment, textFile);
                        break;
                    //if the user selects option 3
                    case "3":
                        DeleteAssessment(assessment, textFile);
                        break;
                    //exit program
                    case "4":
                        exit = true;
                        break;
                    //if the user enters invalid input
                    default:
                        Console.WriteLine("Invalid option. Please try again. ");
                        break;

                }//end switch
                Console.WriteLine("Thanks for using the program");

            }//while end

            //Display(dataFiles);
        }


        /*
         * Method 1 LoadAssessmentsFromFile     
         * Input Requirements: textfile
         * Output Expections: checks if file exists and creates list from file
         */
        static List<string> LoadAssessmentsFromFile(string filePath)
        {
            List<string> AssessmentList = new List<string>();

            //If the file already exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not Found \n Please press 2 to add assessment"); 
            }
            else
                    {
                // Create a StreamReader object
                using (StreamReader reader = new StreamReader(filePath))
                {
                    //Keep going until we reach the end of file
                    while (!reader.EndOfStream)
                    {
                        //Read each line
                        string aline = reader.ReadLine();
                        //If there is data on the line
                        if (!string.IsNullOrWhiteSpace(aline))
                        {
                            //Add it as an item to our list
                            AssessmentList.Add(aline);
                        }
                    }
                }
            } 
            return AssessmentList;
        }//end LoadAssessmentsFromFile


        /*
         * Method 2 DisplayAssessment
         * Input Requirements: assessment string
         * Output Expections: display assessments on the assessments list
         */
        static void DisplayAssessment(List<string> assessments)
        {
            //if the list is empty
            if (assessments.Count == 0)
            {
                Console.WriteLine("No Assessments recorded.");
            }
            else
            {
                //loop through the list
                for (int i = 0; i < assessments.Count; i++)
                {
                    // Each assessment is stored as "Unit/Cluster|Assessment Name|Assessment Type|Due Date"
                    string[] parts = assessments[i].Split('|'); //split on the pipe (|)
                    if (parts.Length == 4) //adjusted from 3 to 4 from practice code
                    {
                        Console.WriteLine($"{i + 1}. Unit/Cluster: {parts[0]}, Assessment Name: {parts[1]}, Assessment Type: {parts[2]}, Due Date: {parts[3]}");
                    }
                }//end loop through the list
            }
        }//end DisplayAssessment


        /*
         * Method 3 AddAssessment
         * Input Requirements: 
         * user input for Unit/Cluster, Name, Type, Due Date.
         * Output Expections: 
         * line is added to assessments list with user inputted data
         * date is verified in the correct format
         */
        static void AddAssessment(List<string> assessments, string filePath)
        {
            // Gets the assessment data from the user
            Console.Write("Enter Unit/Cluster Name: ");
            string? unitCluster = Console.ReadLine();
           
            Console.Write("Enter Assessment Name: ");
            string? assessmentName = Console.ReadLine();

            Console.Write("Enter Assessment Type: ");
            string? assessmentType = Console.ReadLine();



            //show use the correct date format
            Console.Write("Enter date (MM-DD-YYYY): ");
            string? dateInput = Console.ReadLine();
            //Validate date format
            if (!DateTime.TryParseExact(dateInput, "mm-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime assessmentDate))
            {
                Console.WriteLine("Invalid date format. ");
                return;
            }
          
            // Format the new assessment entry and add it to the list.
            string newAssessment = $"{unitCluster}|{assessmentName}|{assessmentType}|{assessmentDate:mm-dd-yyyy}";
            assessments.Add(newAssessment);

            //Write the updated assessment list back to the file.
            WriteAssessmentsToFile(assessments, filePath);

            Console.WriteLine("Assessment added successfully!");

        }//end AddAssessment


        /*
         * Method 4 DeleteAssessment
         * Input Requirements: user (int) input
         * Output Expections: delete line based off user input number
         */
        static void DeleteAssessment(List<string> assessments, string filePath)
        {

            Console.Write("Enter the number of the assessment to delete: ");
            if (int.TryParse(Console.ReadLine(), out int assessmentNumber))
            {

                if((assessmentNumber > 0) && (assessmentNumber <= assessments.Count))
                {
                    Console.WriteLine("The file is being deteted....");
                    assessments.RemoveAt(assessmentNumber - 1);
                    WriteAssessmentsToFile(assessments, filePath);
                    Console.WriteLine("Assessment deleted succesfully!");
                }
                else
                {
                    Console.WriteLine("Invalid Assessment number although the file exist.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Assessment number.");
            }
        }//end DeleteAssessment


        /*
         * Method 5 WriteAssessmentToFile
         * Input Requirements: assessmentList
         * Output Expections: Writes the current list of assessments back to the file using StreamWriter
         */
        static void WriteAssessmentsToFile(List<string> assessmentList, string filePath)
        {
            // Create a StreamWriter object
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string assessment in assessmentList)
                {
                    writer.WriteLine(assessment);
                }
            }

        }//end WriteAssessmentToFile
    }
}
