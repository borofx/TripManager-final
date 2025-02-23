Installation

Clone the repository
git clone https://github.com/borofx/TripManager-final

Navigate to the project directory
cd TripManager

Update the database connection string in appsettings.json
  "ConnectionStrings": {
    "DefaultConnection": "Server=**YOUR SERVER**;Database=TripManagerDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },

Apply database migrations
Update-database

Run the application
