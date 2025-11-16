# Authentication & Authorization Setup Guide

## Understanding OAuth 2.0 vs OpenID Connect

### OAuth 2.0
**Purpose**: Authorization framework - "Can this app access my resources?"
- Focuses on **authorization** (permissions)
- Allows apps to access resources on behalf of users
- Example: "Allow this app to read my Google Drive files"

### OpenID Connect (OIDC)
**Purpose**: Authentication layer built on OAuth 2.0 - "Who is this user?"
- Adds **authentication** (identity) on top of OAuth 2.0
- Provides user identity information (ID token)
- Example: "This user is john@example.com"

### Why Use OpenID Connect?
For authentication in your API, you want **OpenID Connect** because:
- It provides JWT tokens with user identity (ID tokens)
- It's built on OAuth 2.0, so you get both auth and authorization
- It's the standard for modern authentication

## Architecture Overview

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Client    │────────▶│ OAuth Server │────────▶│  Your API   │
│  (Browser/  │         │  (Keycloak)  │         │  (ASP.NET)  │
│   Mobile)   │◀────────│   in Docker  │◀────────│             │
└─────────────┘         └──────────────┘         └─────────────┘
    1. Login               2. Issue JWT             3. Validate
    4. Get Token           5. Return Token          JWT & Allow
```

## Implementation Choices

### OAuth Server Options
1. **Keycloak** (Recommended for this setup)
   - ✅ Free, open-source
   - ✅ Easy Docker setup
   - ✅ Full OAuth 2.0 + OIDC support
   - ✅ User management UI included
   - ✅ Production-ready

2. **Duende IdentityServer** (Alternative)
   - ✅ .NET native
   - ❌ Commercial license required for production
   - ❌ More complex setup

### JWT Token Flow
1. User logs in → OAuth server validates credentials
2. OAuth server issues JWT token (contains user info + permissions)
3. Client sends JWT token in `Authorization: Bearer <token>` header
4. Your API validates JWT signature and extracts user info
5. API grants/denies access based on token claims

## Implementation Steps

✅ **Step 1**: Set up Keycloak in Docker
✅ **Step 2**: Configure JWT authentication in ASP.NET Core
✅ **Step 3**: Create protected API endpoints
✅ **Step 4**: Test the authentication flow

## Configuration Details

### Keycloak Setup
- Port: 8080 (default)
- Admin console: http://localhost:8080/admin
- Realm: Create a realm for your application
- Client: Register your API as a client
- Users: Create test users

### API Configuration
- JWT validation against Keycloak's public key
- Token validation on each request
- Claims extraction for authorization

## Next Steps
See the implementation files created in this project:
- `docker-compose.yml` - Keycloak server setup
- `backend/CleanArch.API/Api.cs` - JWT authentication configuration
- `backend/CleanArch.API/appsettings.json` - JWT settings
