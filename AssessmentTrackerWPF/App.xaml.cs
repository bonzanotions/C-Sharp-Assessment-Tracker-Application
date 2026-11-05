/*
 * Project: Assessment Tracker
 * File: App.xaml.cs
 *
 * Version: 0.2.0
 *
 * Description: The application class. Creates the shared assessment manager
 * and loads the assessment files when the application starts.
 *
 * Contains: App class (Manager property, OnStartup method)
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 29 / 10 / 2026
 * Date modified: 30 / 10 / 2026
 */

using System.IO;
using System.Windows;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The one manager shared by every window.
        /// </summary>
        public static AssessmentManager Manager { get; } = new AssessmentManager();

        /// <summary>
        /// Loads the assessment files before the first window opens.
        /// </summary>
        /// <param name="e">The startup event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Manager.LoadAll();
            }
            catch (IOException)
            {
                MessageBox.Show("The assessment file could not be read.");
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// Saves both assessment files when the application closes.
        /// </summary>
        /// <param name="e">The exit event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                Manager.SaveAll();
            }
            catch (IOException)
            {
                MessageBox.Show("The assessments could not be saved.");
            }

            base.OnExit(e);
        }
    }
}
