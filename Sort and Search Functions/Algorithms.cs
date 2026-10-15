namespace Sort_and_Search_Functions
{
    public class Algorithms
    {
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


        /* Selection sort is a simple sorting algorithm that works by repeatedly finding the minimum element from the unsorted part of the list and putting it at the beginning. 
         * The algorithm maintains two subarrays in a given array: 
         * 1) The subarray which is already sorted. 
         * 2) The remaining subarray which is unsorted.
         */ 
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

        /* Linear search is a simple search algorithm that checks every element in a list sequentially until the desired element is found or the list ends. 
         * It is also known as sequential search.
         */
        public static int Finding(string s, string[] phone)
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
