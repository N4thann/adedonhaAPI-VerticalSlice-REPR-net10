# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

AdedonhaWeb — frontend for [AdedonhaAPI](../adedonhaAPI-VerticalSlice-REPR-net10-main): a word
repository for the game Adedonha (Stop!). Public Catalog module (browse categories/words) plus a
JWT-protected Admin module (CRUD, upload CSV, AboutSite). Requires the API running locally (default
`http://localhost:5055`) — set `VITE_API_BASE_URL` in `.env` (see `.env.example`).

## Commands

```bash
npm run dev       # dev server (localhost:5173)
npm run build     # tsc -b + vite build (type-check gates the build)
npm run lint      # ESLint
npm test          # vitest run (single run)
npm run test:watch
```

Per this user's global instructions, `npm run dev` is always run manually by the user. `npm run
build` and `npm test` may be run as a final verification step after implementing something.

## Architecture

**Stack**: React + TypeScript + Vite (`@vitejs/plugin-react-swc`), MUI, React Router, Zustand, Axios,
Vitest + Testing Library.

**Layering** — each domain concept (e.g. `category`, `word`, `aboutSite`) repeats the same vertical
slice: `types/{domain}.types.ts` → `services/{domain}Service.ts` (Axios calls) →
`store/{domain}/use{Domain}Store.ts` (Zustand, owns loading/error state and calls the service) →
pages/components that consume the store. Follow this slice shape when adding a new domain instead of
inventing a new pattern. Unlike the reference frontend (LoteriasWeb), there is no "module" concept —
AdedonhaAPI is a single domain (categories/words).

**Component organization** is Atomic Design: `components/atoms|molecules|organisms|templates`. Pages
live under `pages/{feature}/` and compose organisms/templates; routing is centralized in
`routes/index.tsx` (`AppRoutes`).

**Cross-cutting state lives in Context, not Zustand** — `contexts/ColorThemeContext` (color theme,
persisted in `localStorage` under `adedonha.colorTheme`; 3 palettes defined in `theme.ts`:
Coral/Roxo/Verde, each with `mode`/`primary`/`secondary`/`background` baked in). `AuthContext` (JWT
session for the Admin module) does not exist yet — added alongside the first Admin screens.

**API requests**: single Axios instance in `services/api.ts`, `baseURL` from `VITE_API_BASE_URL`. No
request interceptor yet — added once `AuthContext` exists and Admin calls need a Bearer token.

**Testing**: Vitest + Testing Library, jsdom environment (`vite.config.ts` / `src/setupTests.ts`).
Convention is one `*.test.ts(x)` colocated next to the file it covers.
