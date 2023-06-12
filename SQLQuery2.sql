SELECT * FROM Roommate rm JOIN Room r ON rm.RoomId = r.Id
SELECT rm.FirstName, COUNT(rm.FirstName) AS 'CountOfChores' FROM Roommate rm JOIN RoommateChore rc ON rm.Id = rc.RoommateId GROUP BY rm.FirstName
