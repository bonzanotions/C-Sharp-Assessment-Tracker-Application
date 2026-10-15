namespace BubbleSorting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] array = { 64, 34, 25, 12, 22, 11, 90 };

            int[] sortedArr = BubbleSort(array);

            foreach (int num in sortedArr)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine("");

            int[] array2 = { 64, 34, 25, 12, 22, 11, 90 };

            int[] selectionSortedArr = Selection(array2);

            foreach (int num in sortedArr)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine("");

            string[] phoneNum = { "0405123456", "0434321456", "0412321654", "0412987654", };
            string[] contactName = { "John Smith", "Jane Doe", "Alice Johnson", "Bob Brown" };
            string[] contactAddress = { "5th Avenue", "Main Street", "Elm Street", "Oak Street" };
            Console.WriteLine("Enter a phone number to search for: ");
            string enteredPhoneNum = Console.ReadLine();
            int searchResult = Finding (enteredPhoneNum, phoneNum);
            if (searchResult != -1)
            {
                Console.WriteLine("Contact found: " + contactName[searchResult] + ", " + contactAddress[searchResult]);
            }
            else
            {
                Console.WriteLine("Contact not found.");
            }

        }
        /* Bubble sort is a simple sorting algorithm that repeatedly steps through the list, 
         * compares adjacent elements and swaps them if they are in the wrong order. 
         * The pass through the list is repeated until the list is sorted.
         */
        static int[] BubbleSort(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        // Swap array[j] and array[j+1]
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
            return arr;
        }
        static int[] Selection(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }
                // Swap array[i] and array[minIndex]
                int temp = arr[minIndex];
                arr[minIndex] = arr[i];
                arr[i] = temp;
            }
            return arr;
        }
        static int Finding (string s, string[] phone)
        {
            for (int i = 0; i < phone.Length; i++)
            {
                if (s.Equals(phone[i]))
                {
                    return i;
                }
               
            }
            return -1;
        }
    }
}
