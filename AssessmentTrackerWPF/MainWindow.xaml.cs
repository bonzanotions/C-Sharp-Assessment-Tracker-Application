/*
 * Project: Asssesment Tracker
 * 
 * Version: 0.1.0
 *
 * Description:  a simple WPF application that stores the details of any 
 * assessments (unit or cluster name, assessment number or name, the type of 
 * assessment and the due date)
 *
 * Contains: Main and ? other Methods
 * 
 * Written by: John Richardson (M196909)
 *
 * Date: 29 / 08 / 2026
 */

using Microsoft.VisualBasic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Load existing from the file using the separate method.
        const string textFile = "assessments.txt";
        List<string> AssessmentList = new List<string>();
        

        public MainWindow()
        {
            InitializeComponent();
            LoadAssessmentsFromFile(textFile);
        }

        /*
         * Method 1 LoadAssessmentsFromFile     
         * Input Requirements: textfile
         * Output Expections: checks if file exists and creates list from file
         */
        private void LoadAssessmentsFromFile(string filePath)
        {
            

            //If the file already exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not Found");
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
           
        }//end LoadAssessmentsFromFile


        private void btnViewAssess_Click(object sender, RoutedEventArgs e)
        {
            ///closes Main window and opens the View Assessments in the same spot.
            wndViewAssess viewAssessWindow = new wndViewAssess();
            viewAssessWindow.Left = this.Left;
            viewAssessWindow.Top = this.Top;
            viewAssessWindow.Show();
            DisplayOnList(AssessmentList, viewAssessWindow.lstViewAssess);

            this.Hide();
        }

        void DisplayOnList(List<string> record, ListView listView)
        {
            foreach (var data in record)
                listView.Items.Add(data);
        }

        private void btnAddAssess_Click(object sender, RoutedEventArgs e)
        {
            ///closes Main window and opens the Add Assessments in the same spot.
            wndAddAssess addAssessWindow = new wndAddAssess();
            addAssessWindow.Left = this.Left;
            addAssessWindow.Top = this.Top;
            addAssessWindow.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        /// <summary>
        /// This button will delete a chosen assessment from the list of assessments.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelAssess_Click(object sender, RoutedEventArgs e)
        {
            
        }
        /// <summary>
        /// This button is currently under construction and will be implemented in the future.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditAssess_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button Under Construction");
        }

    }
}