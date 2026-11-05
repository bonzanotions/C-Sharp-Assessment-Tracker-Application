/*
 * Project: Assessment Tracker
 * File: AssessmentManager.cs
 *
 * Version: 0.4.0
 *
 * Description: Holds the lists of current and completed assessments and
 * loads them from their text files.
 *
 * Contains: AssessmentManager class (LoadAll, AddAssessment, LoadFromFile,
 * ParseLine, NumberCurrentAssessments, DeleteAnyAssessment, DeleteAssessment, DeleteCompleted)
 *
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 30 / 10 / 2026
 * Date modified: 31 / 10 / 2026
 */

using System.Globalization;
using System.IO;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Manages the current and completed assessment lists and their text files.
    /// </summary>
    public class AssessmentManager
    {
        // File names and the date format used in the text files
        private const string CURRENT_FILE = "assessments.txt";
        private const string COMPLETED_FILE = "completed.txt";
        private const string DATE_FORMAT = "MM-dd-yyyy";
        private const string COMPLETED_GRADE = "S";
        //The next ID to assign to a new assessment
        private int nextId = 1;

        /// <summary>
        /// Gets the assessments that are not yet completed.
        /// </summary>
        public List<Assessment> CurrentAssessments { get; private set; } = new List<Assessment>();

        /// <summary>
        /// Gets the assessments graded S, kept in order of ID.
        /// </summary>
        public List<Assessment> CompletedAssessments { get; private set; } = new List<Assessment>();

        /// <summary>
        /// Loads both text files into their lists and numbers the current assessments.
        /// </summary>
        public void LoadAll()
        {
            LoadFromFile(CURRENT_FILE, CurrentAssessments);
            LoadFromFile(COMPLETED_FILE, CompletedAssessments);
            CurrentAssessments.Sort((first, second) => first.DueDate.CompareTo(second.DueDate));
            AssignMissingIds();
        }

        /// <summary>
        /// Works out the next unused ID, then gives an ID to any assessment that doesn't have one yet.
        /// </summary>
        private void AssignMissingIds()
        {
            // Step 1: find the highest ID already in use across both lists
            int highestId = 0;

            foreach (Assessment assessment in CurrentAssessments)
            {
                if (assessment.Id > highestId)
                {
                    highestId = assessment.Id;
                }
            }

            foreach (Assessment assessment in CompletedAssessments)
            {
                if (assessment.Id > highestId)
                {
                    highestId = assessment.Id;
                }
            }

            nextId = highestId + 1;

            // Step 2: give a new ID to anything that has none (Id of 0)
            foreach (Assessment assessment in CurrentAssessments)
            {
                if (assessment.Id == 0)
                {
                    assessment.Id = nextId;
                    nextId++;
                }
            }

            foreach (Assessment assessment in CompletedAssessments)
            {
                if (assessment.Id == 0)
                {
                    assessment.Id = nextId;
                    nextId++;
                }
            }
        }

        /// <summary>
        /// Gives a new assessment an ID and adds it to the list that matches its grade.
        /// </summary>
        /// <param name="newAssessment">The assessment to add.</param>
        public void AddAssessment(Assessment newAssessment)
        {
            newAssessment.Id = nextId;
            nextId++;
            PlaceAssessment(newAssessment);
        }

        /// <summary>
        /// Puts an assessment in the list that matches its grade, in the correct position,
        /// without changing its ID.
        /// </summary>
        /// <param name="assessment">The assessment to place.</param>
        private void PlaceAssessment(Assessment assessment)
        {
            int position = 0;

            if (assessment.Grade == COMPLETED_GRADE)
            {
                // Completed list: keep it in order of ID
                while (position < CompletedAssessments.Count &&
                       CompletedAssessments[position].Id < assessment.Id)
                {
                    position++;
                }

                CompletedAssessments.Insert(position, assessment);
            }
            else
            {
                // Current list: keep it in order of due date
                while (position < CurrentAssessments.Count &&
                       CurrentAssessments[position].DueDate <= assessment.DueDate)
                {
                    position++;
                }

                CurrentAssessments.Insert(position, assessment);
            }
        }

        /// <summary>
        /// Changes the details of an assessment and moves it to the list that matches its new grade.
        /// The ID is not changed.
        /// </summary>
        /// <param name="assessmentToUpdate">The assessment to change.</param>
        /// <param name="unitCluster">The new unit/cluster.</param>
        /// <param name="assessmentName">The new assessment name.</param>
        /// <param name="assessmentType">The new assessment type.</param>
        /// <param name="dueDate">The new due date.</param>
        /// <param name="grade">The new grade.</param>
        /// <returns>True if the assessment was found and updated, otherwise false.</returns>
        public bool UpdateAssessment(Assessment assessmentToUpdate, string unitCluster,
                                     string assessmentName, string assessmentType,
                                     DateTime dueDate, string grade)
        {
            // Take it out of whichever list it is in
            if (!DeleteAnyAssessment(assessmentToUpdate))
            {
                return false;
            }

            // Change its details (the ID stays the same)
            assessmentToUpdate.UnitCluster = unitCluster;
            assessmentToUpdate.AssessmentName = assessmentName;
            assessmentToUpdate.AssessmentType = assessmentType;
            assessmentToUpdate.DueDate = dueDate;
            assessmentToUpdate.Grade = grade;

            // Put it back in the right list, in the right position
            PlaceAssessment(assessmentToUpdate);
            return true;
        }

        /// <summary>
        /// Removes an assessment from the current list.
        /// </summary>
        /// <param name="assessmentToDelete">The assessment to remove.</param>
        /// <returns>True if the assessment was found and removed, otherwise false.</returns>
        public bool DeleteAssessment(Assessment assessmentToDelete)
        {
            // Look through the list for the assessment, position by position
            for (int index = 0; index < CurrentAssessments.Count; index++)
            {
                if (CurrentAssessments[index] == assessmentToDelete)
                {
                    CurrentAssessments.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        ///<summary>
        /// Savesthe current and completed assessments to their respective text files.
        /// </summary>
        public void SaveAll()
        {
            SaveToFile(CURRENT_FILE, CurrentAssessments, true);
            SaveToFile(COMPLETED_FILE, CompletedAssessments, true);
        }

        ///<summary>
        /// Writes every assessment in the given list to the specified text file, one per line.
        /// </summary>
        /// <param name="filePath">The text file to write to.</param>
        /// <param name="sourceList">The list of assessments to write.</param>
        /// <param name="includeId">Whether to include the ID in the output.</param>
        private void SaveToFile(string filePath, List<Assessment> sourceList, bool includeId)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Assessment assessment in sourceList)
                {
                    writer.WriteLine(BuildLine(assessment, includeId));
                }
            }
        }

        /// <summary>
        /// Builds a line of text for one assessment, optionally including the ID.
        /// </summary>
        /// <param name="assessment">The assessment to write.</param>
        /// <param name="includeId">Whether to include the ID in the output.</param>
        /// <returns>The line of text, with the parts separated by | characters.</returns>
        private string BuildLine(Assessment assessment, bool includeId)
        {
            string dateText = assessment.DueDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

            string line = string.Join("|", assessment.UnitCluster, assessment.AssessmentName,
                                      assessment.AssessmentType, dateText, assessment.Grade);

            if (includeId)
            {
                line = assessment.Id + "|" + line;
            }

            return line;
        }

        /// <summary>
        /// Reads a text file line by line and adds each valid line to the given list.
        /// </summary>
        /// <param name="filePath">The text file to read.</param>
        /// <param name="targetList">The list to add the assessments to.</param>
        private void LoadFromFile(string filePath, List<Assessment> targetList)
        {
            // Nothing to load if the file does not exist yet
            if (!File.Exists(filePath))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                // Keep reading until the end of the file
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();

                    // Skip blank lines
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Assessment? assessment = ParseLine(line);

                        // Only keep lines that were valid
                        if (assessment != null)
                        {
                            targetList.Add(assessment);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Turns one line of a text file into an assessment.
        /// </summary>
        /// <param name="line">The line to read.</param>
        /// <returns>The assessment, or null if the line is not valid.</returns>
        private Assessment? ParseLine(string line)
        {
            List<string> parts = line.Split('|').Select(part => part.Trim()).ToList();

            // Make every line 6 parts: ID, unit, name, type, date, grade
            if (parts.Count == 4)
            {
                parts.Insert(0, "0");
                parts.Add("NYM");
            }
            else if (parts.Count == 5)
            {
                parts.Insert(0, "0");
            }
            else if (parts.Count != 6)
            {
                return null;
            }

            // The ID must be a whole number
            if (!int.TryParse(parts[0], out int id))
            {
                return null;
            }

            // The date must match the format exactly
            // Use InvariantCulture to avoid issues with different locales
            if (!DateTime.TryParseExact(parts[4], DATE_FORMAT, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime dueDate))
            {
                return null;
            }

            // The grade must be one of the three allowed values
            string grade = parts[5].ToUpper();
            if (grade != "NYM" && grade != "NYS" && grade != "S")
            {
                return null;
            }

            return new Assessment(id, parts[1], parts[2], parts[3], dueDate, grade);
        }

        /// <summary>
        /// Joins all the details of an assessment into one lowercase string, used for searching.
        /// </summary>
        /// param name="assessment">The assessment to join.</param>
        /// <returns>A single string containing all the assessment's details.</returns>
        private string GetSearchText(Assessment assessment)
        {
            return string.Join(" ", assessment.UnitCluster, assessment.AssessmentName,
                               assessment.AssessmentType, assessment.DueDate.ToString(DATE_FORMAT),
                               assessment.Grade).ToLower();
        }

        /// <summary>
        /// Searches the current and completed assessments and returns the best matches first.
        /// </summary>
        /// <param name="searchText">The text to look for. Empty text returns everything.</param>
        /// <returns>The matching assessments, ordered from closest match to loosest match.</returns>
        public List<Assessment> SearchAssessments(string searchText)
        {
            string search = searchText.Trim().ToLower();

            // Put both lists together so one search covers them
            List<Assessment> allAssessments = new List<Assessment>(CurrentAssessments);
            allAssessments.AddRange(CompletedAssessments);

            if (search == "")
            {
                return allAssessments;
            }

            List<Assessment> exact = new List<Assessment>();
            List<Assessment> contains = new List<Assessment>();
            List<Assessment> ordered = new List<Assessment>();
            List<Assessment> containsAll = new List<Assessment>();
            List<Assessment> partial = new List<Assessment>();

            foreach (Assessment assessment in allAssessments)
            {
                string text = GetSearchText(assessment);

                // Group 1: one whole field matches exactly
                if (assessment.UnitCluster.ToLower() == search ||
                    assessment.AssessmentName.ToLower() == search ||
                    assessment.AssessmentType.ToLower() == search ||
                    assessment.Grade.ToLower() == search)
                {
                    exact.Add(assessment);
                    continue;
                }

                // Group 2: the text contains what was typed
                if (text.Contains(search))
                {
                    contains.Add(assessment);
                    continue;
                }

                // Group 3: the typed characters appear in the same order
                int searchIndex = 0;
                foreach (char letter in text)
                {
                    if (searchIndex < search.Length && letter == search[searchIndex])
                    {
                        searchIndex++;
                    }
                }

                if (searchIndex == search.Length)
                {
                    ordered.Add(assessment);
                    continue;
                }

                // Group 4: the text contains every typed character, in any order
                bool hasAll = true;
                foreach (char letter in search)
                {
                    if (!text.Contains(letter))
                    {
                        hasAll = false;
                        break;
                    }
                }

                if (hasAll)
                {
                    containsAll.Add(assessment);
                    continue;
                }

                // Group 5: at least half of the typed characters are in the text
                int matches = 0;
                foreach (char letter in search)
                {
                    if (text.Contains(letter))
                    {
                        matches++;
                    }
                }

                if (matches > 0 && matches >= search.Length / 2)
                {
                    partial.Add(assessment);
                }
            }

            // Join the groups together, best matches first
            List<Assessment> results = new List<Assessment>();
            results.AddRange(exact);
            results.AddRange(contains);
            results.AddRange(ordered);
            results.AddRange(containsAll);
            results.AddRange(partial);

            return results;
        }

        /// <summary>
        /// Finds a completed assessment by its ID using a binary search.
        /// The completed list must be in order of ID.
        /// </summary>
        /// <param name="id">The ID to look for.</param>
        /// <returns>The assessment, or null if no completed assessment has that ID.</returns>
        public Assessment? FindCompletedById(int id)
        {
            int low = 0;
            int high = CompletedAssessments.Count - 1;

            // Keep narrowing the range until it is empty
            while (low <= high)
            {
                int middle = (low + high) / 2;
                int middleId = CompletedAssessments[middle].Id;

                if (middleId == id)
                {
                    return CompletedAssessments[middle];
                }
                else if (middleId < id)
                {
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether another assessment already has the same unit/cluster, name and type.
        /// Upper/lower case and extra spaces at the ends are ignored.
        /// </summary>
        /// <param name="unitCluster">The unit/cluster to check.</param>
        /// <param name="assessmentName">The assessment name to check.</param>
        /// <param name="assessmentType">The assessment type to check.</param>
        /// <param name="assessmentToIgnore">An assessment to skip (the one being edited), or null.</param>
        /// <returns>True if a matching assessment exists, otherwise false.</returns>
        public bool IsDuplicate(string unitCluster, string assessmentName, string assessmentType,
                                Assessment? assessmentToIgnore)
        {
            // Put both lists together so one loop covers them
            List<Assessment> allAssessments = new List<Assessment>(CurrentAssessments);
            allAssessments.AddRange(CompletedAssessments);

            foreach (Assessment assessment in allAssessments)
            {
                // Skip the assessment that is being edited
                if (assessment == assessmentToIgnore)
                {
                    continue;
                }

                if (assessment.UnitCluster.Trim().ToLower() == unitCluster.Trim().ToLower() &&
                    assessment.AssessmentName.Trim().ToLower() == assessmentName.Trim().ToLower() &&
                    assessment.AssessmentType.Trim().ToLower() == assessmentType.Trim().ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes an assessment from the completed list.
        /// </summary>
        /// <param name="assessmentToDelete">The assessment to remove.</param>
        /// <returns>True if the assessment was found and removed, otherwise false.</returns>
        public bool DeleteCompleted(Assessment assessmentToDelete)
        {
            for (int index = 0; index < CompletedAssessments.Count; index++)
            {
                if (CompletedAssessments[index] == assessmentToDelete)
                {
                    CompletedAssessments.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes an assessment from whichever list it is in.
        /// </summary>
        /// <param name="assessmentToDelete">The assessment to remove.</param>
        /// <returns>True if the assessment was found and removed, otherwise false.</returns>
        public bool DeleteAnyAssessment(Assessment assessmentToDelete)
        {
            if (DeleteAssessment(assessmentToDelete))
            {
                return true;
            }

            return DeleteCompleted(assessmentToDelete);
        }
    }
}