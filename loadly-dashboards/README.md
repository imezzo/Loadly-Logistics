# Loadly Frontend

A modern Vue 3 frontend application for logistics and distribution management, built with Vite, TypeScript, and Tailwind CSS.

## 📋 Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Multi-Platform Architecture](#multi-platform-architecture)
- [Internationalization (i18n)](#internationalization-i18n)
- [Development Features](#development-features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Environment Variables](#environment-variables)
- [Available Scripts](#available-scripts)
- [Project Structure](#project-structure)
- [Development](#development)
- [Building for Production](#building-for-production)
- [Deployment](#deployment)
- [Code Quality](#code-quality)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

Loadly is a comprehensive logistics and distribution platform featuring multiple dashboards:
- **Admin Dashboard**: Administrative interface for system management
- **App Dashboard**: Web user interface
  > **Note**: This dashboard will be modified in the future based on business requirements
- **Company Dashboard**: Company-specific management interface

## 🏗 Multi-Platform Architecture

The application uses a sophisticated platform-based architecture that builds three separate dashboards from a single codebase:

### Platform Resolution
- Utilizes a `platformComponentResolver` to dynamically load platform-specific components
- Platform-specific components are organized in dedicated directories (`admin-dashboard/`, `app-dashboard/`, `platform-dashboard/`)
- Environment variables determine the active platform mode

### Environment Modes
- Development: Each platform runs on its own port
- Production: Each platform builds to its own output directory
- Shared code is maintained in common directories while platform-specific overrides are possible

## 🌐 Internationalization (i18n)

The application supports multiple languages using `vue-i18n`:

### Supported Languages
- English (en)
- Arabic (ar) - RTL support included

### Adding New Translations
1. Navigate to `src/locales/`
2. Choose the appropriate platform directory (`admin-dashboard/`, `app-dashboard/`, `platform-dashboard/`)
3. Add or modify TS or JSON files for each type and language to be `[type].[lang].[ts or json]` (e.g., `forms.en.ts`, `forms.ar.ts`)
4. The system will automatically load the appropriate translations based on dashboard mode.

### Usage in Components
```vue
<template>
  <div>{{ $t('title.greeting') }}</div>
</template>
```

## ⚙️ Development Features

### Path Aliases
- `@/` - Points to `src/` directory
- `@/components/` - Shared components
- `@/views/` - Page components

Example usage:
```typescript
import MyComponent from '@/components/MyComponent.vue'
```

### Auto-Imports
Vue 3 Composition API and other utilities are auto-imported, so you don't need to manually import them:
- Vue APIs: `ref`, `computed`, `watch`, etc.
- Vue Router: `useRoute`, `useRouter`
- Pinia: `storeToRefs`, `defineStore`
- VueUse utilities

Just use them directly in your components:
```typescript
// No need to import ref or computed
const count = ref(0)
const double = computed(() => count.value * 2)
```

## 🛠 Technology Stack

- **Framework**: [Vue 3](https://vuejs.org/) (Composition API)
- **Build Tool**: [Vite](https://vitejs.dev/)
- **Language**: [TypeScript](https://www.typescriptlang.org/)
- **Styling**: [Tailwind CSS v4](https://tailwindcss.com/)
- **State Management**: [Pinia](https://pinia.vuejs.org/)
- **Routing**: [Vue Router](https://router.vuejs.org/)
- **Form Validation**: [VeeValidate](https://vee-validate.logaretm.com/)
- **Internationalization**: [Vue I18n](https://vue-i18n.intlify.dev/)
- **HTTP Client**: [Axios](https://axios-http.com/)
- **Utilities**: [VueUse](https://vueuse.org/)

## ✅ Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js**: `v24.11.1` (specified in `.nvmrc`)
  - **Using nvm (recommended)**:
    ```bash
    nvm install
    nvm use
    ```
  - **Without nvm**: Install [Node.js v24.11.1](https://nodejs.org/) directly
- **npm**: Latest version (comes with Node.js)
- **Git**: For cloning the repository

## 📥 Installation

### 1. Clone the Repository

```bash
git clone https://github.com/Loadly-EG/loadly-dashboards.git
cd loadly-dashboards
```

### 2. Install Dependencies

```bash
npm install
```

### 3. Configure Environment Variables

Create the appropriate `.env` files based on your target platform (see [Environment Variables](#environment-variables) section).

## 🔐 Environment Variables

The project uses different environment files for each dashboard mode:

### `.env.admin` - Admin Dashboard
```env
BASE_URL=localhost:3000
PORT=3000
```

### `.env.company-app` - Company App Dashboard
```env
BASE_URL=localhost:3000
PORT=3000
```

### `.env.company-platform` - Company Platform Dashboard
```env
BASE_URL=localhost:3000
PORT=3000
```

**Note**: Update these values according to your backend API endpoints and deployment configuration.

## 📜 Available Scripts

### Development

Run all dashboards concurrently:
```bash
npm run dev
```

Run individual dashboards:
```bash
# Admin Dashboard
npm run dev:admin

# App Dashboard
npm run dev:app

# Company Dashboard
npm run dev:company
```

### Production Build

Build all dashboards:
```bash
npm run build
```

Build individual dashboards:
```bash
# Admin Dashboard → dist/admin-dashboard
npm run build:admin

# App Dashboard → dist/app-dashboard
npm run build:app

# Company Dashboard → dist/company-dashboard
npm run build:company
```

### Preview Production Build

Preview all dashboards:
```bash
npm run preview
```

Preview individual dashboards:
```bash
# Admin Dashboard
npm run preview:admin

# App Dashboard
npm run preview:app

# Company Dashboard
npm run preview:company
```

### Code Quality

Type checking:
```bash
npm run type-check
```

Linting (with auto-fix):
```bash
npm run lint
```

## 📁 Project Structure

```
loadly-dashboards/
├── public/              # Static assets
├── src/
│   ├── assets/           # Static assets
│   │   ├── css/          # Global CSS files
│   │   ├── fonts/        # Font files (en/ar)
│   │   ├── images/       # Image assets
│   │   └── svg/          # SVG icons and illustrations
│   │
│   ├── components/       # Reusable Vue components
│   │   ├── base/         # Base UI components
│   │   └── icon/         # Icon components
│   │
│   ├── composables/      # Vue 3 composables
│   ├── locale/           # i18n translations
│   │   ├── admin-dashboard/  # Admin translations
│   │   ├── app-dashboard/    # App translations
│   │   └── company-dashboard/# Company translations
│   │
│   ├── plugins/          # Vue plugins
│   │
│   ├── router/           # Vue Router configuration
│   │   ├── admin/        # Admin routes
│   │   ├── app/          # App routes
│   │   ├── platform/     # Platform routes
│   │   ├── index.ts      # Main router configuration
│   │   └── routesResolver.ts # Route resolution logic
│   │
│   ├── stores/           # Pinia stores
│   │   ├── admin/        # Admin store modules
│   │   ├── app/          # App store modules
│   │   └── company/      # Company store modules
│   │
│   ├── types/            # TypeScript type definitions
│   │   └── platform.ts   # Platform-specific types
│   │
│   └── main.ts           # Application entry point
│
├── .env                  # Shared environment variables
├── .env.admin            # Admin dashboard env
├── .env.company-app      # Company app env
├── .env.company-platform # Company platform env
├── .eslint.config.ts     # ESLint configuration
├── .nvmrc               # Node.js version
├── commitlint.config.js  # Commit message linting
├── index.html           # HTML entry point
├── package.json         # Project dependencies and scripts
├── tsconfig.app.json    # TypeScript app config
├── tsconfig.json        # TypeScript base config
├── tsconfig.node.json   # TypeScript Node.js config
└── vite.config.ts       # Vite configuration
```

## 🚀 Development

### Recommended IDE Setup

- **Editor**: [VS Code](https://code.visualstudio.com/)
- **Extensions**:
  - [Vue - Official](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (disable Vetur if installed)
  - [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)
  - [Tailwind CSS IntelliSense](https://marketplace.visualstudio.com/items?itemName=bradlc.vscode-tailwindcss)
  - [SonarQube for IDE](https://marketplace.visualstudio.com/items?itemName=SonarSource.sonarlint-vscode)
  - [Stylelint](https://marketplace.visualstudio.com/items?itemName=stylelint.vscode-stylelint)

### Recommended Browser Setup

**Chromium-based browsers** (Chrome, Edge, Brave):
- [Vue.js devtools](https://chromewebstore.google.com/detail/vuejs-devtools/nhdogjmejiglipccpnnnanhbledajbpd)

**Firefox**:
- [Vue.js devtools](https://addons.mozilla.org/en-US/firefox/addon/vue-js-devtools/)

### Development Workflow

1. Start the development server for your target dashboard
2. Make your changes
3. The browser will automatically reload with HMR (Hot Module Replacement)
4. Run linting before committing: `npm run lint`
5. Commit your changes (Husky will run pre-commit hooks)

## 🏗 Building for Production

### Build Process

The build process includes:
1. Linting all files
2. Building all three dashboards
3. Optimizing assets
4. Generating production-ready bundles

```bash
npm run build
```

**Build Output Directories:**

When running `npm run build`, all three dashboards will be built into separate directories:
- `dist/admin-dashboard/` - Admin Dashboard production build
- `dist/app-dashboard/` - App Dashboard production build
- `dist/company-dashboard/` - Company Dashboard production build

### Preview Production Build

After building, you can preview the production build locally:

```bash
npm run preview
```

**Note:** When running `npm run preview`, all three dashboards will run concurrently on different ports. Each dashboard will be served on its own port as configured in the respective environment files.

## 🔍 Code Quality

### Linting

The project uses ESLint with the following configurations:
- Vue 3 recommended rules
- TypeScript support
- Tailwind CSS best practices
- SonarJS for code quality

### Stylelint

The project uses Stylelint for CSS/SCSS linting with:
- Standard CSS rules
- Vue-specific CSS configurations

### EditorConfig

The project includes an `.editorconfig` file to maintain consistent coding styles across different editors and IDEs. This ensures consistent indentation, line endings, and other formatting rules.

### Type Checking

TypeScript is configured with strict mode. Run type checking:

```bash
npm run type-check
```

### Git Hooks

The project uses Husky for Git hooks:
- **Pre-commit**: Runs linting on staged files
- **Commit-msg**: Validates commit messages using Commitlint

### Commitlint

All commit messages must follow the [Conventional Commits](https://www.conventionalcommits.org/) specification. Commits that don't follow this format will be rejected.

**Format:**
```
<type>(<scope>): <subject>
```

**Examples:**
```bash
feat(auth): add login functionality
fix(dashboard): resolve navigation bug
docs(readme): update installation instructions
chore(deps): upgrade vue to v3.5.25
```

**Common types:** `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `perf`, `ci`, `build`

## 📝 Additional Notes

### Type Support for `.vue` Imports

TypeScript cannot handle type information for `.vue` imports by default. This project uses `vue-tsc` for type checking and requires the Volar extension in your editor.

### Auto Imports

The project uses `unplugin-auto-import` and `unplugin-vue-components` for automatic imports of:
- Vue APIs (ref, computed, watch, etc.)
- Vue Router APIs
- Pinia APIs
- Custom components

