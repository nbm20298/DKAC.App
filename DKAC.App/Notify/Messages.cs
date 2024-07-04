namespace DKAC.App.Notify
{
    public static class Messages
    {
        public static class Notifications
        {
            public static string Add(string title)
            {
                return $"The {title} has been added successfully.";
            }
            public static string Update(string title)
            {
                return $"The {title} has been updated successfully.";
            }
            public static string Delete(string title)
            {
                return $"The {title} has been deleted successfully.";
            }
            public static string UndoDelete(string title)
            {
                return $"The {title} has been retrieved successfully.";
            }
            public static string LogoutComplete()
            {
                return $"You have been logout successfully!";
            }
        }
    }
}
