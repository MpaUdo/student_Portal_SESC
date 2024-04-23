# Student Portal Microservice

The Student Portal Microservices project aims to create a robust and scalable web application for managing student-related tasks within an educational institution. Leveraging microservices architecture, this system provides various features to enhance the student experience. Below is an overview of the key functionalities:

## Features

1. **Register/Login**:
   - Users can create a portal account by registering with their credentials.
   - Upon successful registration, users can log in securely to access the portal.

2. **View Courses**:
   - Students can explore all available courses offered by the institution.
   - Course details, such as title, description,etc, are displayed.

3. **Enrol in Course**:
   - Students can enrol in specific courses of interest.

4. **View Enrolments**:
   - Students can see a list of courses they are currently enrolled in.
   - Enrolment status, course codes etc.

5. **View/Update Student Profile**:
   - Students can view their profile, including their unique student ID.
   - They have the option to update their name and surname as needed.

6. **Graduation Eligibility**:
   - The system checks if a student is eligible for graduation.
   - Eligibility criteria include having no outstanding invoices or fees.

## Microservices Architecture

The Student Portal is built using microservices architecture, which allows for modular development, scalability, and independent deployment of services. Key components include:

- **Authentication Service**: Handles user registration, login, and token-based authentication.
- **Course Service**: Manages course information, enrolments, and course-related operations.
- **Student Service**: Stores and retrieves student profiles, including personal details.

## Technologies Used

- **ASP.NET Core**: For building RESTful APIs and microservices.
- **Docker**: Containerization for easy deployment and scalability.
- **PostgreSQL**: As the database for storing student profiles, course data, and invoices.
- **JWT (JSON Web Tokens)**: For secure authentication and authorization.
- **Swagger**: API documentation and testing.

## Getting Started


To set up and run the project, follow these steps:

1. Clone the repository to your local machine.
2. Ensure that Docker is installed and running.
3. Open a terminal or command prompt and navigate to the project directory.
4. Run the following command to build and start the Docker containers:

   ```
   docker-compose up -d
   ```

   This command will build the Docker images and start the containers specified in the Docker Compose file.

5. Wait for the containers to start. You can monitor the container logs to see if there are any errors:

   ```
   docker-compose logs -f
   ```

6. Once the containers are up and running, you can access the Student Portal microservice Swagger documentation at http://localhost:8000/swagger. There's also a postman doc included in the Api project layer of the project.

Feel free to explore the APIs and test the functionality provided by the microservices.

Cheers!
