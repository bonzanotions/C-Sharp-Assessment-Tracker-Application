/*
 * Project: Assessment Tracker
 * File: Assessment.cs
 *
 * Version: 0.2.0
 *
 * Description: Represents a single assessment. Stores the unit or cluster
 * name, assessment name, assessment type, due date, grade and ID number.
 *
 * Contains: Assessment class (properties and constructor)
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 30 / 10 / 2026
 * Date modified: 30 / 10 / 2026
 */

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Represents a single assessment and stores its details.
    /// </summary>
    public class Assessment
    {
        /// <summary>
        /// Gets or sets the unique ID number of the assessment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unit or cluster name, for example C# One.
        /// </summary>
        public string UnitCluster { get; set; }

        /// <summary>
        /// Gets or sets the assessment name, for example AT2.3.
        /// </summary>
        public string AssessmentName { get; set; }

        /// <summary>
        /// Gets or sets the assessment type, for example Project.
        /// </summary>
        public string AssessmentType { get; set; }

        /// <summary>
        /// Gets or sets the due date of the assessment.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the grade: NYM (Not Yet Marked), NYS (Not Yet Satisfactory) or S (Satisfactory).
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Creates a new assessment with the given details.
        /// </summary>
        /// <param name="id">The unique ID number.</param>
        /// <param name="unitCluster">The unit or cluster name.</param>
        /// <param name="assessmentName">The assessment name.</param>
        /// <param name="assessmentType">The assessment type.</param>
        /// <param name="dueDate">The due date.</param>
        /// <param name="grade">The grade. Defaults to NYM.</param>
        public Assessment(int id, string unitCluster, string assessmentName, string assessmentType,
                          DateTime dueDate, string grade)
        {
            Id = id;
            UnitCluster = unitCluster;
            AssessmentName = assessmentName;
            AssessmentType = assessmentType;
            DueDate = dueDate;
            Grade = grade;
        }
    }
}
