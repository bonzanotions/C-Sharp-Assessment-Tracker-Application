/*
 * Project: Assessment Tracker (WPF)
 * Date created: 22/10/2026
 * Date modified: 22/10/2026
 * Version: 1.0.0
 * Author: John Richardson (M196909)
 * Description: Holds the two data structures (current and completed assessments) and the standard algorithms
 *              that work on them: sequential search, binary search, insertion and deletion. Also loads and
 *              saves the data files. Both lists are always kept sorted by unit then name so that binary
 *              search can be used on them.
 */

namespace AssessmentTracker
{
    /// <summary>
    /// Manages the lists of current and completed assessments.
    /// </summary>
    public class AssessmentManager
    {
        /// <summary>Assessments that are not yet graded S (sorted by unit then name).</summary>
        public List<Assessment> Current { get; } = new List<Assessment>();

        /// <summary>Assessments graded S (sorted by unit then name).</summary>
        public List<Assessment> Completed { get; } = new List<Assessment>();

        /// <summary>
        /// Compares two assessments by unit and then by name, ignoring case.
        /// </summary>
        /// <param name="unitA">Unit of the first assessment.</param>
        /// <param name="nameA">Name of the first assessment.</param>
        /// <param name="unitB">Unit of the second assessment.</param>
        /// <param name="nameB">Name of the second assessment.</param>
        /// <returns>Negative if A sorts first, zero if equal, positive if B sorts first.</returns>
        private static int CompareKeys(string unitA, string nameA, string unitB, string nameB)
        {
            int result = string.Compare(unitA, unitB, StringComparison.OrdinalIgnoreCase);
            if (result == 0)
            {
                result = string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        /// <summary>
        /// Sequential (linear) search. Checks every assessment in turn and keeps the ones whose chosen field
        /// contains the search text. An empty search text returns every assessment.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="field">"Unit", "Type" or "Name".</param>
        /// <param name="searchText">Text to look for (not case sensitive).</param>
        /// <returns>A new list of the matching assessments.</returns>
        public static List<Assessment> SequentialSearch(List<Assessment> list, string field, string searchText)
        {
            List<Assessment> matches = new List<Assessment>();

            for (int i = 0; i < list.Count; i++)
            {
                string value = list[i].Unit;
                if (field == "Type")
                {
                    value = list[i].Type;
                }
                else if (field == "Name")
                {
                    value = list[i].Name;
                }

                if (value.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(list[i]);
                }
            }

            return matches;
        }

        /// <summary>
        /// Binary search. Repeatedly halves the sorted list to find an assessment by unit and name.
        /// </summary>
        /// <param name="sortedList">A list sorted by unit then name.</param>
        /// <param name="unit">Unit to find.</param>
        /// <param name="name">Assessment name to find.</param>
        /// <returns>The index of the assessment, or -1 if it is not in the list.</returns>
        public static int BinarySearch(List<Assessment> sortedList, string unit, string name)
        {
            int low = 0;
            int high = sortedList.Count - 1;

            while (low <= high)
            {
                int middle = low + (high - low) / 2;
                int comparison = CompareKeys(sortedList[middle].Unit, sortedList[middle].Name, unit, name);

                if (comparison == 0)
                {
                    return middle;
                }
                else if (comparison < 0)
                {
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Insertion. Finds the correct position in the sorted list and inserts the assessment there so
        /// the list stays sorted.
        /// </summary>
        /// <param name="sortedList">A list sorted by unit then name.</param>
        /// <param name="item">The assessment to insert.</param>
        /// <returns>The index where the assessment was inserted.</returns>
        public static int Insert(List<Assessment> sortedList, Assessment item)
        {
            int position = 0;

            while (position < sortedList.Count
                && CompareKeys(sortedList[position].Unit, sortedList[position].Name, item.Unit, item.Name) < 0)
            {
                position++;
            }

            sortedList.Insert(position, item);
            return position;
        }

        /// <summary>
        /// Deletion. Uses binary search to find the assessment and then removes it.
        /// </summary>
        /// <param name="sortedList">A list sorted by unit then name.</param>
        /// <param name="item">The assessment to remove.</param>
        /// <returns>True if the assessment was found and removed.</returns>
        public static bool Delete(List<Assessment> sortedList, Assessment item)
        {
            int index = BinarySearch(sortedList, item.Unit, item.Name);

            if (index < 0)
            {
                return false;
            }

            sortedList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Adds a new assessment. S-graded assessments go to the completed list, all others to the current list.
        /// </summary>
        /// <param name="item">The assessment to add.</param>
        /// <param name="message">Explains the problem if the assessment could not be added.</param>
        /// <returns>True if it was added.</returns>
        public bool TryAdd(Assessment item, out string message)
        {
            if (BinarySearch(Current, item.Unit, item.Name) >= 0 || BinarySearch(Completed, item.Unit, item.Name) >= 0)
            {
                message = $"{item.Unit} {item.Name} is already in the tracker.";
                return false;
            }

            Insert(ListFor(item), item);
            message = "";
            return true;
        }

        /// <summary>
        /// Replaces an assessment with an edited copy. If the new grade is S it ends up in the completed list,
        /// otherwise in the current list (so changing S back to NYS moves it back).
        /// </summary>
        /// <param name="original">The assessment being edited.</param>
        /// <param name="updated">The edited details.</param>
        /// <param name="message">Explains the problem if the update was refused.</param>
        /// <returns>True if the update was made.</returns>
        public bool TryUpdate(Assessment original, Assessment updated, out string message)
        {
            foreach (List<Assessment> list in new[] { Current, Completed })
            {
                int index = BinarySearch(list, updated.Unit, updated.Name);
                if (index >= 0 && !ReferenceEquals(list[index], original))
                {
                    message = $"{updated.Unit} {updated.Name} is already in the tracker.";
                    return false;
                }
            }

            List<Assessment> source = Completed.Contains(original) ? Completed : Current;
            Delete(source, original);
            Insert(ListFor(updated), updated);
            message = "";
            return true;
        }

        /// <summary>
        /// Chooses the list an assessment belongs in, based on its grade.
        /// </summary>
        /// <param name="item">The assessment.</param>
        /// <returns>The completed list for grade S, otherwise the current list.</returns>
        private List<Assessment> ListFor(Assessment item)
        {
            return item.Grade == Assessment.GRADE_SATISFACTORY ? Completed : Current;
        }

        /// <summary>
        /// Loads both lists from their files. A missing file just gives an empty list. Lines that cannot be
        /// read are skipped and counted.
        /// </summary>
        /// <param name="currentPath">Path of the current assessments file.</param>
        /// <param name="completedPath">Path of the completed assessments file.</param>
        /// <returns>The number of lines that were skipped, or -1 if neither file exists.</returns>
        public int LoadFromFiles(string currentPath, string completedPath)
        {
            if (!File.Exists(currentPath) && !File.Exists(completedPath))
            {
                return -1;
            }

            Current.Clear();
            Completed.Clear();

            int skipped = LoadFile(currentPath);
            skipped += LoadFile(completedPath);
            return skipped;
        }

        /// <summary>
        /// Reads one file and adds each valid line to the correct list.
        /// </summary>
        /// <param name="path">The file to read.</param>
        /// <returns>The number of lines skipped.</returns>
        private int LoadFile(string path)
        {
            int skipped = 0;

            if (!File.Exists(path))
            {
                return skipped;
            }

            foreach (string line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Skip lines that are not valid, and duplicates of an assessment already loaded
                if (Assessment.TryParse(line, out Assessment item) && TryAdd(item, out _))
                {
                    continue;
                }
                skipped++;
            }

            return skipped;
        }

        /// <summary>
        /// Writes both lists to their files, one assessment per line.
        /// </summary>
        /// <param name="currentPath">Path of the current assessments file.</param>
        /// <param name="completedPath">Path of the completed assessments file.</param>
        public void SaveToFiles(string currentPath, string completedPath)
        {
            File.WriteAllLines(currentPath, Current.Select(item => item.ToFileLine()));
            File.WriteAllLines(completedPath, Completed.Select(item => item.ToFileLine()));
        }
    }
}
