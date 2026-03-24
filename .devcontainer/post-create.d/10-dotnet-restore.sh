#!/usr/bin/env bash
set -euo pipefail

# Restore NuGet packages for both .NET projects.
dotnet restore src/api/SupportTicketAgent/SupportTicketAgent.csproj
dotnet restore src/ui/SupportTicketAgent.UI/SupportTicketAgent.UI.csproj
