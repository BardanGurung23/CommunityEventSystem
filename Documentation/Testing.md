# Testing

## Testing Strategy

Testing was carried out to confirm that the Community Event Management System works correctly for both administrators and participants. The testing focused on authentication, role-based access, event browsing, participant registration, admin management features, reporting, validation, and service-level business logic.

The application was tested using a combination of build testing, automated xUnit tests, and manual functional testing. Automated tests were used for service-layer behaviour, while manual testing was used for full Blazor user workflows involving navigation, authentication state, forms, and database changes.

## Test Environment

| Item | Details |
|---|---|
| Framework | ASP.NET Core Blazor Web App |
| .NET SDK | .NET 8 |
| Database | MySQL |
| ORM | Entity Framework Core with Pomelo MySQL provider |
| Authentication | Cookie authentication with Admin and Participant roles |
| Browser testing | Local browser using `dotnet run` |
| Build command | `dotnet build` |
| Automated testing | xUnit with EF Core InMemory provider |
| Test command | `dotnet test` |

## Build Test

| Test | Expected Result | Actual Result | Status |
|---|---|---|---|
| Run `dotnet build` | Project should compile without errors | Build succeeded with 0 errors and 0 warnings | Pass |

## Automated Test Results

| Test | Expected Result | Actual Result | Status |
|---|---|---|---|
| Run `dotnet test` | Automated tests should compile and pass | 12 tests passed, 0 failed, 0 skipped | Pass |

## Automated Test Coverage

| Test Area | Coverage |
|---|---|
| Event service | Valid event creation, past event rejection, invalid time rejection, upcoming event filtering |
| Registration service | Participant registration, duplicate registration prevention, confirm registration, cancel registration |
| Report service | Event registration counts excluding cancelled registrations |
| Venue service | Venue availability update |
| Activity service | Activity deactivation |
| Authentication support | Participant password hashing and verification |

## Manual Functional Test Cases

| Test ID | Area | Test Case | Steps | Expected Result | Actual Result | Status |
|---|---|---|---|---|---|---|
| T01 | Authentication | Admin login with valid credentials | Open Login page, expand admin login, enter `admin@community.test` and `admin123` | Admin is logged in and redirected to dashboard | Admin dashboard opened | Pass |
| T02 | Authentication | Participant sign up | Open Login page, select Sign Up, enter valid participant details and password | Participant account is created and user is redirected to Sign In | Account created message displayed on login page | Pass |
| T03 | Authentication | Participant login with valid credentials | Enter participant email and password | Participant is logged in and redirected to My Registrations | My Registrations page opened | Pass |
| T04 | Authentication | Invalid participant login | Enter wrong email or password | Login fails and error message is displayed | Error message displayed | Pass |
| T05 | Role access | Logged-out user accessing admin page | Open `/admin/dashboard` while logged out | User is blocked and asked to login | Login warning displayed | Pass |
| T06 | Role access | Participant accessing admin page | Login as participant and open `/admin/dashboard` | Participant is not allowed to access admin area | Access blocked | Pass |
| T07 | Events | Browse upcoming events | Open Events page | Active upcoming events are displayed as cards | Event cards displayed | Pass |
| T08 | Events | Filter events by date | Select a date and click Filter | Events for selected date are shown | Filtered event list displayed | Pass |
| T09 | Events | Filter events by venue | Enter venue name and click Filter | Events using that venue are shown | Filtered event list displayed | Pass |
| T10 | Events | Filter events by activity type | Enter activity type and click Filter | Matching events are shown | Filtered event list displayed | Pass |
| T11 | Events | Clear event filters | Apply filters, then click Clear | Filters reset and upcoming events return | Upcoming events displayed again | Pass |
| T12 | Event Details | View event details | Click View details on an event card | Event date, time, venue, activities, capacity, and registration panel are shown | Details page displayed correctly | Pass |
| T13 | Registration | Participant registers for event | Login as participant, open event details, click Register me | Registration is created and confirmation message appears | Registration submitted message displayed | Pass |
| T14 | Registration | Prevent duplicate registration | Register for the same event twice | System prevents duplicate registration | Duplicate registration error shown | Pass |
| T15 | Participant Area | View own registrations | Login as participant and open My Registrations | Participant registrations are listed | Registrations displayed | Pass |
| T16 | Participant Area | Cancel own registration | Click Cancel on a registration | Registration status is updated/cancelled | Registration cancelled | Pass |
| T17 | Admin Events | Add event | Login as admin, open Manage Events, click Add event, complete dialog, save | Event is added and table refreshes | Event added successfully | Pass |
| T18 | Admin Events | Edit event | Click Edit on an event, update details in dialog, save | Event is updated and table refreshes | Event updated successfully | Pass |
| T19 | Admin Events | Deactivate event | Click Deactivate on an active event | Event status becomes inactive | Event deactivated | Pass |
| T20 | Admin Venues | Add venue | Open Venues, click Add venue, complete dialog, save | Venue is created | Venue displayed in table | Pass |
| T21 | Admin Venues | Edit venue | Click Edit, update venue details, save | Venue details are updated | Venue updated successfully | Pass |
| T22 | Admin Venues | Toggle venue availability | Click Toggle on a venue | Availability changes between Yes and No | Availability updated | Pass |
| T23 | Admin Activities | Add activity | Open Activities, click Add activity, complete dialog, save | Activity is created | Activity displayed in table | Pass |
| T24 | Admin Activities | Edit activity | Click Edit, update activity details, save | Activity details are updated | Activity updated successfully | Pass |
| T25 | Admin Activities | Deactivate activity | Click Deactivate | Activity becomes inactive | Activity deactivated | Pass |
| T26 | Admin Participants | Add participant | Open Participants, click Add participant, complete dialog, save | Participant is created | Participant displayed in table | Pass |
| T27 | Admin Participants | Edit participant | Click Edit, update participant details, save | Participant details are updated | Participant updated successfully | Pass |
| T28 | Admin Participants | Deactivate participant | Click Deactivate | Participant becomes inactive | Participant deactivated | Pass |
| T29 | Admin Registrations | Approve registration | Open Registrations and click Approve | Registration status becomes Confirmed | Status updated | Pass |
| T30 | Admin Registrations | Cancel registration | Click Cancel | Registration status becomes Cancelled | Status updated | Pass |
| T31 | Reports | View reports dashboard | Open Reports page as admin | Event, venue, and activity reports are displayed with totals and bars | Reports displayed correctly | Pass |
| T32 | Navigation | Sidebar auth state | Login and logout using different roles | Sidebar changes between Login, Logout, participant links, and admin links | Correct links displayed | Pass |
| T33 | Validation | Invalid event time | Add event with start time after end time | System rejects event and shows validation/error message | Error displayed | Pass |
| T34 | Validation | Past event date | Add event using a past date | System rejects event | Error displayed | Pass |

## Security and Access Testing

| Area | Test | Result |
|---|---|---|
| Password handling | Participant passwords are stored as hashes using `PasswordHasher<Participant>` | Pass |
| Admin-only pages | Admin pages use `[Authorize(Roles = AuthConstants.AdminRole)]` | Pass |
| Participant pages | Participant-only pages require participant authentication | Pass |
| Logout | Logout clears the authentication cookie | Pass |

## Database Testing

| Test | Expected Result | Actual Result | Status |
|---|---|---|---|
| EF Core migration runs | Database schema is created/updated automatically | `Database.MigrateAsync()` runs during seeding | Pass |
| Seed data inserts | Initial venues, activities, participants, and events are added when database is empty | Seed data available | Pass |
| Event relationships | Events can be linked to venues and activities | Linked data appears in event list/details | Pass |
| Registration uniqueness | Participant cannot register for the same event more than once | Duplicate registration prevented | Pass |

## Usability Testing

| Test | Result |
|---|---|
| Login and sign-up are available from one page | Pass |
| Admin add/edit actions use dialogs instead of separate pages | Pass |
| Event list is card-based and easy to scan | Pass |
| Event details clearly show date, time, venue, activities, and registration action | Pass |
| Reports and dashboard provide visual summaries | Pass |
| Sidebar icons make navigation easier to understand | Pass |

## Known Limitations

- Admin login uses fixed demo credentials rather than a full database-backed admin account system.
- Password reset and email verification are not implemented.
- Social login and OTP buttons are not implemented because they require external services.

## Testing Conclusion

The testing confirmed that the main functional requirements and key service-layer behaviours of the Community Event Management System work correctly. Administrators can manage events, venues, activities, participants, registrations, and reports. Participants can create an account, login, browse events, register for events, and manage their registrations. Role-based access control prevents unauthorised access to admin features, and the application builds successfully without errors.
