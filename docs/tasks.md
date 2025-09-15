# Improvement Tasks Checklist

1. [x] Introduce an application service layer (e.g., IMangaService/ILightNovelService/IFilmService/IBaseObjectService) so UI depends on abstractions instead of Infrastructure repositories; register services via DI. 
2. [x] Define repository/service interfaces in a separate abstractions namespace/project (e.g., MyLittleLibrary.Application or .Domain.Abstractions) to decouple Infrastructure from UI.
3. [x] Refactor UI components to inject service interfaces (e.g., IMangaService/IBaseObjectService) rather than concrete repositories (MangaRepository/BaseObjectRepository).
4. [x] Move heavy code-behind logic from .razor files into .razor.cs partial classes to improve separation of concerns and testability.
5. [x] Introduce a query/command separation (lightweight CQRS) for reads vs writes to clarify data flows and reduce side effects.
6. [x] Add cancellation token support to all async repository/service methods and propagate from UI calls.
7. [ ] Add pagination/slicing parameters (skip/take) to list-returning methods (e.g., GetAll, GetAllByTitle) to avoid loading large collections in memory. 
10. [ ] Create MongoDB indexes: Title (case-insensitive), TitleSlug, CollectionType, and compound unique index (Title, Volume) for Manga; add index creation on startup. 
8. [x] Centralize error handling and user notifications (e.g., a NotificationService) instead of ad-hoc Snackbar.Add calls scattered in components. 
9. [x] Add structured logging using ILogger<T> in repositories and services; log at appropriate levels with contextual metadata (title, id, volume). 
11. [ ] Replace regex title search with indexed, case-insensitive search (collation) or prefix search to leverage indexes and improve performance. 
12. [ ] Ensure BaseObject.Timestamp is consistently set by domain factories/constructors and is UTC; avoid resetting timestamps during updates unless intended (add UpdatedAt if needed). 
13. [ ] Add validation for domain invariants (e.g., Title non-empty, Volume > 0 for Manga, TitleSlug conforms to slug rules) using constructors or a validation layer. 
14. [ ] Normalize TitleSlug generation in a single utility/service to guarantee consistency across creates/updates and prevent mismatches. 
15. [ ] Review nullability: enable Nullable Reference Types across projects and annotate properties/parameters; fix or suppress warnings appropriately. 
16. [ ] Enable .NET SDK analyzers and StyleCop (or similar) with a repo-wide ruleset; set TreatWarningsAsErrors in CI to improve code quality. 
17. [ ] Replace magic strings and numbers with constants/enums where appropriate (e.g., image folder names, dialog titles). 
18. [ ] Unify repeated repository code (FilmRepository, LightNovelRepository, MangaRepository) via shared base patterns (e.g., specification pattern or generic repository with discriminator). 
19. [ ] Add unit tests for domain models and invariants (e.g., Manga volume validation, slug generation). 
20. [ ] Add integration tests for repositories using Testcontainers for MongoDB; assert CRUD behavior and index usage. 
21. [ ] Add bUnit tests for critical UI components (MangaInfo, Mangas page, dialogs) covering rendering, state changes, and interactions. 
22. [ ] Introduce a thin ViewModel layer for UI binding to avoid mutating domain records directly from components. 
23. [ ] Extract image/file handling from BookUploadDialog to a dedicated FileStorageService with: size/type validation, sanitization, collision handling, and safe path generation. 
24. [ ] Enforce file type allowlist (e.g., .jpg, .png, .webp) and max size limits configurable via appsettings; reject unsafe extensions and validate content-type signatures. 
25. [ ] Sanitize and normalize file names (remove unsafe characters, enforce TitleSlug prefix) and protect against path traversal when saving files. 
26. [ ] Generate optimized thumbnails and consider WebP conversion; store both original and thumbnail paths for faster listing. 
27. [ ] Add virtualization for long lists/grids (MudVirtualize) to improve UI performance on large collections. 
28. [ ] Improve accessibility: ensure all images have meaningful alt text; add aria-labels; ensure overlay supports Esc to close, focus trapping, and keyboard navigation. 
29. [ ] Fix copy/wording issues (e.g., change "Readed" to "Read") and ensure consistent terminology across the app. 
30. [ ] Introduce a consistent, reusable card/list component for series/items to reduce duplication between Manga/LightNovels/Films pages. 
31. [ ] Add defensive checks where lists may be empty (e.g., OpenAddVolumeDialog assumes Last() exists) to avoid exceptions on new series. 
32. [ ] Avoid fetching entire sets when only aggregates are needed; add repository methods for max volume, counts, or summaries. 
33. [ ] Add health checks (MongoDB connectivity) and expose a health endpoint; integrate with container readiness/liveness if containerized. 
34. [ ] Externalize secrets (MongoDB connection string) via environment variables or user-secrets; ensure appsettings.json contains placeholders only. 
35. [ ] Add environment-specific appsettings (Development, Production) and enable options validation (ValidateDataAnnotations, ValidateOnStart). 
36. [ ] Add CI pipeline (e.g., GitHub Actions) to build, test, run analyzers, and publish artifacts (or container images). 
37. [ ] Add basic OpenTelemetry instrumentation (logs/traces) and structured logging format (JSON) for production diagnostics. 
38. [ ] Introduce caching for frequently accessed computed data (e.g., series summaries) with invalidation on writes. 
39. [ ] Add global exception handling for the Blazor app (ErrorBoundary usage) and user-friendly error pages/alerts. 
40. [ ] Add user documentation: README with architecture overview, setup steps, environment variables, and data directory conventions. 
41. [ ] Provide a developer guide for adding new collection types (e.g., comics) and reusing shared components/services. 
42. [ ] Add data seeding/import scripts and backup strategy for MongoDB collections; document migration steps for new indexes. 
43. [ ] Review and standardize date handling and formatting (UTC vs local, display formats) across components. 
44. [ ] Ensure consistent DI registration in Program.cs for repositories/services; prefer AddScoped for data services in Blazor Server. 
45. [ ] Add guards and retries around MongoDB operations where appropriate (transient errors), using driver retry policies. 
46. [ ] Replace direct string comparisons for Title with case-insensitive normalized comparisons or TitleSlug usage to avoid mismatches. 
47. [ ] Add server-side input validation for query parameters (e.g., Title from query string) and guard against injection patterns in regex filters. 
48. [ ] Consider introducing a small API boundary (Minimal API) for data access if planning to support alternative clients; keep UI as a consumer. 
49. [ ] Implement auditing fields where needed (CreatedAt, UpdatedAt) and set them consistently on create/update operations. 
50. [ ] Add code ownership and PR templates to ensure reviews check accessibility, performance, security, and test coverage.
