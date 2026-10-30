/*
 *Project: Asssesment Tracker
 * View Assessments Window
 * 
 * Version: 0.1.0
 *
 *Description: A window that displays the list of assessments in a ListView control.
 *
 * Contains: Main and ? other Methods
 * 
 * Written by: John Richardson (M196909)
 *
 *Date: 29 / 08 / 2026
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for wndViewAssess.xaml
    /// </summary>
    public partial class wndViewAssess : Window
    {
        public wndViewAssess()
        {
            InitializeComponent();
            /*
             * Method  DisplayAssessment
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
        }

        private void lstViewAssess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            ///closes View Assessments window and opens the Main Menu in the same spot.
            MainWindow mainWindow = new MainWindow();
            
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }
    }
}
