using Roommates.Models;
using Roommates.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Roommates
{
    class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true;TrustServerCertificate=true;";

        static void Main(string[] args)
        {
            // resository often refers to a class that performs data access for a specific object, separate one for each type of object we have
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
            RoommateRepository roomieRepo = new RoommateRepository(CONNECTION_STRING);

            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for room"):
                        Console.Write("Room Id: ");
                        int id = int.Parse(Console.ReadLine());

                        Room room = roomRepo.GetById(id);

                        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a room"):
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Update a room"):
                        List<Room> roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to update? ");
                        int selectedRoomId = int.Parse(Console.ReadLine());
                        Room selectedRoom = roomOptions.FirstOrDefault(r => r.Id == selectedRoomId);

                        Console.Write("New Name: ");
                        selectedRoom.Name = Console.ReadLine();

                        Console.Write("New Max Occupancy: ");
                        selectedRoom.MaxOccupancy = int.Parse(Console.ReadLine());

                        roomRepo.Update(selectedRoom);

                        Console.WriteLine("Room has been successfully updated");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Delete a room"):
                        List<Room> roomsToDelete = roomRepo.GetAll();
                        foreach (Room r in roomsToDelete)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name} Max Occupancy({r.MaxOccupancy})");
                        }

                        Console.Write("Which room would you like to delete? ");
                        int deletedRoomId = int.Parse(Console.ReadLine());
                        roomRepo.Delete(deletedRoomId);

                        Console.WriteLine("Room has been successfully deleted");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;



                    case ("Show all chores"):
                        List<Chore> chores = choreRepo.GetAll();
                        foreach(Chore c in chores)
                        {
                            Console.WriteLine($"{c.Name} has an Id of {c.Id}");
                        }
                        Console.WriteLine("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Search for chore"):
                        Console.WriteLine("Chore id: ");
                        int choreId = int.Parse(Console.ReadLine());

                        Chore chore = choreRepo.GetById(choreId);

                        Console.WriteLine($"{chore.Id} - {chore.Name}");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Show unassigned chores"):
                        List<Chore> unassignedChores = choreRepo.GetUnassignedChores();
                        foreach(Chore c in unassignedChores)
                        {
                            Console.WriteLine($"{c.Name} has an Id of {c.Id} and is unassigned");
                        }
                        Console.WriteLine("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Add a chore"):
                        Console.WriteLine("Chore name: ");
                        string choreName = Console.ReadLine();

                        Chore choreToAdd = new Chore()
                        {
                            Name = choreName
                        };

                        choreRepo.Insert(choreToAdd);

                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Update a chore"):
                        List<Chore> choreOptions = choreRepo.GetAll();
                        foreach (Chore ch in choreOptions)
                        {
                            Console.WriteLine($"{ch.Id} = {ch.Name}");
                        }

                        Console.WriteLine("\nWhich chore would you like to update?");
                        int selectedChoreId = int.Parse(Console.ReadLine());
                        Chore selectedChore = choreOptions.FirstOrDefault(ch => ch.Id == selectedChoreId);

                        Console.Write("New Name: ");
                        selectedChore.Name = Console.ReadLine();

                        choreRepo.Update(selectedChore);

                        Console.WriteLine("\nSelected chore has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Assign a chore"):
                        List<Chore> chorelist = choreRepo.GetAll();
                        foreach (Chore c in chorelist)
                        {
                            Console.WriteLine($"{c.Name} has an Id of {c.Id}");
                        }
                        Console.WriteLine("\nSelect the chore to assign");
                        int assignedChoreId = int.Parse(Console.ReadLine());
                        
                        List<Roommate> roommates = roomieRepo.GetAll();
                        foreach (Roommate rm in roommates)
                        {
                            Console.WriteLine($"{rm.FirstName} {rm.LastName} has an Id of {rm.Id}");
                        }
                        Console.WriteLine("\nSelect the roommate who should do the chore");
                        int assignedRoommieId = int.Parse(Console.ReadLine());

                        choreRepo.AssignChore(assignedRoommieId, assignedChoreId);

                        Console.WriteLine($"The chore was assigned");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Delete a chore"):
                        List<Chore> choresToDelete = choreRepo.GetAll();
                        foreach (Chore c in choresToDelete)
                        {
                            Console.WriteLine($"{c.Id} - {c.Name}");
                        }

                        Console.Write("Which chore would you like to delete? ");
                        int deletedChoreId = int.Parse(Console.ReadLine());
                        choreRepo.Delete(deletedChoreId);

                        Console.WriteLine("Chore has been successfully deleted");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;

                    case ("Get chore counts"):
                        List<NumOfChores> choreNums = choreRepo.GetChoreCounts();
                        foreach (NumOfChores noc in choreNums)
                        {
                            Console.WriteLine($"{noc.FirstName}: {noc.CountOfChores}");
                        }
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;


                    case ("Search for a roommate"):
                        Console.WriteLine("Roommate id: ");
                        int roommateId = int.Parse(Console.ReadLine());

                        Roommate roommate = roomieRepo.GetById(roommateId);

                        Console.WriteLine($"{roommate.Id} - {roommate.FirstName} pays {roommate.RentPortion} and lives in {roommate.Room.Name}");
                        Console.Write("\nPress any key to continue");
                        Console.ReadKey();
                        break;


                    case ("Exit"):
                        runProgram = false;
                        break;
                }
            }

        }

        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                "Show all rooms",
                "Search for room",
                "Add a room",
                "Update a room",
                "Delete a room",
                "Show all chores",
                "Search for chore",
                "Show unassigned chores",
                "Add a chore",
                "Update a chore",
                "Assign a chore",
                "Delete a chore",
                "Get chore counts",
                "Search for a roommate",                
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }



    }
}