/*
 * Project: Assessment Tracker (WPF)
 * Date created: 22/10/2026
 * Date modified: 22/10/2026
 * Version: 1.0.0
 * Author: John Richardson (M196909)
 * Description: Model class for a single assessment. Knows how to turn itself into a line of text for the data
 *              file and how to be rebuilt from a line of text.
 */

using System.Globalization;

namespace AssessmentTracker
{
    /// <summary>
    /// Holds the details of one assessment.
    /// </summary>
    public class Assessment
    {
        /// <summary>Date format used on screen and in the data files (MM-dd-yyyy).</summary>
        public const string DATE_FORMAT = "MM-dd-yyyy";

        /// <summary>Separator between the fields on a line of the data file.</summary>
        public const char FIELD_SEPARATOR = '|';

        /// <summary>Grade for an assessment that has not been marked yet.</summary>
        public const string GRADE_NOT_MARKED = "NYM";

        /// <summary>Grade for an assessment that is not yet satisfactory.</summary>
        public const string GRADE_NOT_YET_SATISFACTORY = "NYS";

        /// <summary>Grade for a satisfactory assessment.</summary>
        public const string GRADE_SATISFACTORY = "S";

        /// <summary>Unit or cluster name, e.g. C# One.</summary>
        public string Unit { get; set; } = "";

        /// <summary>Assessment name, e.g. AT2.3.</summary>
        public string Name { get; set; } = "";

        /// <summary>Assessment type, e.g. Project.</summary>
        public string Type { get; set; } = "";

        /// <summary>Due date of the assessment.</summary>
        public DateTime DueDate { get; set; }

        /// <summary>Grade: NYM, NYS or S.</summary>
        public string Grade { get; set; } = GRADE_NOT_MARKED;

        /// <summary>Due date as text in MM-dd-yyyy format (used by the grid).</summary>
        public string DueDateText
        {
            get { return DueDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Converts the assessment into one line for the data file.
        /// </summary>
        /// <returns>Unit|Name|Type|Date|Grade</returns>
        public string ToFileLine()
        {
            return string.Join(FIELD_SEPARATOR, Unit, Name, Type, DueDateText, Grade);
        }

        /// <summary>
        /// Tries to rebuild an assessment from a line of the data file. Lines from the console version of the
        /// app have only four fields, so the grade defaults to NYM.
        /// </summary>
        /// <param name="line">The line read from the file.</param>
        /// <param name="assessment">The assessment, if the line was valid.</param>
        /// <returns>True if the line was valid.</returns>
        public static bool TryParse(string line, out Assessment assessment)
        {
            assessment = new Assessment();
            string[] parts = line.Split(FIELD_SEPARATOR);

            if (parts.Length != 4 && parts.Length != 5)
            {
                return false;
            }

            if (!DateTime.TryParseExact(parts[3], DATE_FORMAT, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime dueDate))
            {
                return false;
            }

            string grade = GRADE_NOT_MARKED;
            if (parts.Length == 5)
            {
                grade = parts[4].Trim().ToUpper();
                if (grade != GRADE_NOT_MARKED && grade != GRADE_NOT_YET_SATISFACTORY && grade != GRADE_SATISFACTORY)
                {
                    return false;
                }
            }

            assessment.Unit = parts[0].Trim();
            assessment.Name = parts[1].Trim();
            assessment.Type = parts[2].Trim();
            assessment.DueDate = dueDate;
            assessment.Grade = grade;

            return assessment.Unit.Length > 0 && assessment.Name.Length > 0;
        }
    }
}
