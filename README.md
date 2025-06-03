Setup & Run
1. Database Setup

Install PostgreSQL and pgAdmin
Open pgAdmin and create a database named study_results
Make sure your PostgreSQL credentials match:

Username: postgres
Password: KingKai$$10



2. Backend (.NET API)
bash# Navigate to project root
cd StudyResults

# Run migrations to create tables
dotnet ef database update

# Start API server
dotnet run
API will run at: http://localhost:5173
3. Frontend (React)
bash# Navigate to frontend folder
cd study-results-frontend

# Install dependencies
npm install

# Start frontend
npm start
Frontend will run at: http://localhost:3000
Usage

Go to http://localhost:3000
Upload a CSV file with columns: ParticipantId, MetricName, MetricValue, Timestamp
View charts and statistics on the dashboard
