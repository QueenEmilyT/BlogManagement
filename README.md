# Blog Management System

A modern, full-stack blog management system built with .NET 9, Blazor WebAssembly, and Entity Framework Core. This application provides a comprehensive platform for managing blog posts, authors, comments, and tags with a clean, responsive user interface.

## 🚀 Features

### Core Functionality
- *Blog Post Management*: Create, read, update, delete, publish, schedule, and archive blog posts
- *Author Management*: Comprehensive author profiles with social media integration
- *Comment System*: Threaded comments with moderation, approval workflow, and reporting
- *Tag System*: Flexible tagging system with many-to-many relationships
- *Content Publishing*: Draft, scheduled publishing, and immediate publishing workflows
- *SEO-Friendly*: Automatic slug generation and URL optimization

### Advanced Features
- *Rich Text Editing*: Advanced content editor for blog posts
- *Image Management*: Featured image support for posts
- *Search & Filtering*: Advanced search capabilities across posts, authors, and tags
- *Grid Views*: Data grids with sorting, filtering, and pagination
- *Responsive UI*: Mobile-friendly interface built with Telerik UI for Blazor
- *API Versioning*: RESTful API with versioning support

## 🏗 Architecture

This application follows Clean Architecture principles with clear separation of concerns:


📁 Internship.Domain/          # Domain entities and business rules
├── Authors/                   # Author entity and value objects
├── Posts/                     # Post entity with publishing logic
├── Comments/                  # Comment entity with threading support
├── Tags/                      # Tag entity
└── Common/                    # Shared domain logic

📁 Internship.Application/     # Application services and DTOs
├── Authors/Services/          # Author business logic
├── Posts/Services/           # Post management services
├── Comments/Services/        # Comment moderation services
└── Tags/Services/           # Tag management services

📁 Internship.Infrastructure/  # Data access and external services
├── Repositories/             # Repository implementations
├── Configurations/          # Entity Framework configurations
└── Migrations/             # Database migrations

📁 Internship.API/            # RESTful API layer
├── Endpoints/               # Minimal API endpoints
└── Program.cs              # API configuration

📁 Internship.Blazor/         # Frontend application
├── Pages/                   # Blazor pages and components
├── Services/               # HTTP client services
└── Components/            # Reusable UI components


## 🛠 Technology Stack

### Backend
- *.NET 9* - Latest .NET framework
- *ASP.NET Core* - Web API framework
- *Entity Framework Core* - ORM for data access
- *SQL Server* - Primary database
- *Minimal APIs* - Lightweight API endpoints
- *API Versioning* - Versioned REST APIs

### Frontend
- *Blazor WebAssembly* - Client-side web framework
- *Telerik UI for Blazor* - Professional UI component library
- *Font Awesome Icons* - Icon library
- *HTTP Client with Polly* - Resilient HTTP communication

### Development Tools
- *Entity Framework Migrations* - Database schema management
- *Swagger/OpenAPI* - API documentation
- *Clean Architecture* - Maintainable code structure

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB, Express, or full version)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

## 🚀 Getting Started

### 1. Clone the Repository
bash
git clone <repository-url>
cd Internship


### 2. Database Setup
bash
# Update connection string in appsettings.json
# Run the provided SQL script to create the database
sqlcmd -S (localdb)\MSSQLLocalDB -i create-database.sql

# Or use Entity Framework migrations
dotnet ef database update --project Internship.Infrastructure --startup-project Internship.API


### 3. Configuration
Update the connection string in Internship.API/appsettings.json:
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=InternshipBlog;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}


### 4. Run the Application
bash
# Start the API (runs on https://localhost:7096)
cd Internship.API
dotnet run

# Start the Blazor client (runs on https://localhost:7178)
cd Internship.Blazor
dotnet run


## 📊 Database Schema

The application uses the following main entities:

- *Authors*: User profiles with social media integration
- *Posts*: Blog posts with publishing workflow
- *Comments*: Threaded comments with moderation
- *Tags*: Categorization system
- *PostTags*: Many-to-many relationship between posts and tags

See create-database.sql for the complete database schema.

## 🔗 API Endpoints

### Posts
- GET /api/v1/posts - Get all posts
- GET /api/v1/posts/{id} - Get post by ID
- POST /api/v1/posts - Create new post
- PUT /api/v1/posts/{id} - Update post
- DELETE /api/v1/posts/{id} - Delete post
- GET /api/v1/posts/author/{authorId} - Get posts by author

### Authors
- GET /api/v1/authors - Get all authors
- GET /api/v1/authors/{id} - Get author by ID
- POST /api/v1/authors - Create new author
- PUT /api/v1/authors/{id} - Update author
- DELETE /api/v1/authors/{id} - Delete author

### Comments
- GET /api/v1/comments/post/{postId} - Get comments for a post
- POST /api/v1/comments - Create new comment
- PUT /api/v1/comments/{id}/approve - Approve comment
- DELETE /api/v1/comments/{id} - Delete comment

### Tags
- GET /api/v1/tags - Get all tags
- POST /api/v1/tags - Create new tag
- GET /api/v1/tags/post/{postId} - Get tags for a post
- POST /api/v1/tags/{tagId}/posts/{postId} - Add tag to post

## 🎨 UI Components

The Blazor frontend includes:
- *Data Grids*: Sortable, filterable grids for all entities
- *Forms*: Comprehensive forms with validation
- *Dialogs*: Modal dialogs for CRUD operations
- *Rich Text Editor*: Content editing capabilities
- *Responsive Layout*: Mobile-friendly design

## 🔧 Development

### Adding New Features
1. Define domain entities in Internship.Domain
2. Create repository interfaces and implementations in Internship.Infrastructure
3. Add application services in Internship.Application
4. Create API endpoints in Internship.API/Endpoints
5. Build UI components in Internship.Blazor/Pages

### Running Migrations
bash
# Add new migration
dotnet ef migrations add <MigrationName> --project Internship.Infrastructure --startup-project Internship.API

# Update database
dotnet ef database update --project Internship.Infrastructure --startup-project Internship.API


## 📝 License

This project is developed as part of an internship program and is intended for educational purposes.

## 🤝 Contributing

This is an internship project. For questions or suggestions, please contact the development team.

## 📞 Support

For technical support or questions about the application, please reach out to the project maintainers.
