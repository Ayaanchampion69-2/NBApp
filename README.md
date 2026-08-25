# NBApp
NBApp is an ice cream shop e-commerce web application built with ASP.NET Core MVC. It supports product browsing, cart and checkout, order management, delivery-address handling by suburb/city, and role-based accounts (Guest, User, Admin).

Tech stack
Framework: ASP.NET Core MVC (.NET 10), Razor Pages for Identity
Database: SQL Server via Entity Framework Core
Auth: ASP.NET Core Identity, with Google and Facebook external login
Payments: Stripe.net, with M-PAiSA (Vodafone Fiji) redirect-based payment integration in progress
Validation: FluentValidation
Email: MailKit (SMTP)
Styling: Tailwind CSS 3.4
Frontend extras: vanilla JS (e.g. home page carousel)
Project structure
NBApp/
├── Areas/
│   ├── Admin/            # Admin-only controllers and views
│   └── Identity/         # Login/register/account Razor Pages, Identity data context
├── Controllers/          # Cart, Categories, City/Suburb, Home, Orders, Product, Reports
├── Migrations/           # EF Core migrations
├── Models/                # Domain entities (Products, Order, Category, ShippingAddress, etc.)
├── Services/              # EmailSender, StripeServices
├── Validators/             # FluentValidation validators
├── ViewComponents/
├── ViewModels/
├── Views/
├── wwwroot/                # Static assets, images, compiled CSS, JS
│   ├── css/                # Tailwind input.css / output.css
│   ├── js/                 # home-carousel.js etc.
│   └── MiscPics/, Images/
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Secret.json  # not committed — local secrets
├── tailwind.config.js
├── Program.cs
└── NBApp.csproj
Core domain model

The main entities and relationships:

Category → Products (one-to-many)
Products → OrderItem (one-to-many)
Order → OrderItem (one-to-many)
NBAppUser (extends IdentityUser) → Order (one-to-many)
Order → ShippingAddress
Suburb → ShippingAddress (one-to-many)
City → Suburb (one-to-many)

CartItem is an in-memory, session-based, [NotMapped] model (not persisted to the database). ProductsDto is a form-binding DTO used for product create/edit views.

Getting started
Prerequisites
.NET 10 SDK
SQL Server (local or remote)
Node.js (for Tailwind CSS build)
1. Clone and restore
bash
git clone https://github.com/Ayaanchampion69-2/NBApp.git
cd NBApp/NBApp
dotnet restore
npm install

M-PAiSA credentials (merchant ID, hash key) should be stored the same way rather than committed to appsettings.json.

3. Apply database migrations
bash
dotnet ef database update

The app also seeds the database and creates a default admin account (admin@DaGoat.com) on startup via IdentityConfig.CreateAdminUserAsync.

4. Build Tailwind CSS

Run from the inner project directory (NBApp/NBApp):

bash
npx tailwindcss -c ./tailwind.config.js -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
5. Run the app
bash
dotnet run
Features
Product catalog with categories, stock tracking, sale pricing, and image uploads
Shopping cart (session-based) and checkout flow
Order management with status tracking (Pending → Processing → Shipped → Delivered / Cancelled)
Delivery address selection by City → Suburb, with per-suburb delivery cost
Role-based access: Guest, User, Admin (Admin area with reporting)
External login via Google and Facebook, plus standard email/password Identity accounts
Stripe payments, with M-PAiSA (Vodafone Fiji) integration in progress
Email notifications via MailKit/SMTP
