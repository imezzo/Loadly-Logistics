import { globalIgnores } from 'eslint/config'
import { defineConfigWithVueTs, vueTsConfigs } from '@vue/eslint-config-typescript'
import pluginVue from 'eslint-plugin-vue'
import eslintParserVue from "vue-eslint-parser"
import eslintPluginBetterTailwindcss from "eslint-plugin-better-tailwindcss"
import sonarjs from 'eslint-plugin-sonarjs'
import eslint from '@eslint/js'
import tseslint from 'typescript-eslint'
import autoImportGlobals from './.eslintrc-auto-import.json'

// To allow more languages other than `ts` in `.vue` files, uncomment the following lines:
// import { configureVueProject } from '@vue/eslint-config-typescript'
// configureVueProject({ scriptLangs: ['ts', 'tsx'] })
// More info at https://github.com/vuejs/eslint-config-typescript/#advanced-setup

export default defineConfigWithVueTs(
  sonarjs.configs.recommended,
  {
    name: 'app/files-to-lint',
    files: ['**/*.{ts,mts,tsx,vue}'],
    plugins: {
      "better-tailwindcss": eslintPluginBetterTailwindcss
    },
    rules: {
      "better-tailwindcss/enforce-consistent-line-wrapping": ["warn", {
        printWidth: 100,
      }],
      'better-tailwindcss/enforce-consistent-class-order': 'warn',
      'better-tailwindcss/enforce-consistent-important-position': 'warn',
      'better-tailwindcss/enforce-shorthand-classes': 'warn',
      'better-tailwindcss/no-duplicate-classes': 'warn',
      'better-tailwindcss/no-deprecated-classes': 'warn',
      'better-tailwindcss/no-unnecessary-whitespace': 'warn',
      'better-tailwindcss/no-conflicting-classes': 'warn',


      'no-empty': ['error', { allowEmptyCatch: true }],
      "no-debugger": "error",
      'no-console': ['warn', { allow: ['info', 'warn', 'error'] }],
      'padding-line-between-statements': [
        'error', {
          blankLine: 'always',
          prev: '*',
          next: 'return',
        },
      ],
      "vue/block-lang": ["error",
        {
          "script": {
            "lang": "ts"
          }
        }
      ],
      "@typescript-eslint/naming-convention": [
        "error",
        {
          "selector": ["function"],
          "format": ["camelCase"],
          "leadingUnderscore": "allow"
        }
      ],
      'sonarjs/todo-tag': 'off',
      'sonarjs/function-return-type': 'off',
      'sonarjs/no-hardcoded-passwords': 'off',
    },
    settings: {
      "better-tailwindcss": {
        entryPoint: "src/assets/css/index.css",
      }
    }
  },
  {
    files: ["**/*.vue"],
    languageOptions: {
      globals: autoImportGlobals.globals,
      parser: eslintParserVue
    }
  },

  globalIgnores([
    '**/dist/**',
    '**/dist-ssr/**',
    '**/coverage/**',
    '**/auto-imports.d.ts',
    '**/components.d.ts',
  ]),

  eslint.configs.recommended,
  tseslint.configs.strict,
  tseslint.configs.stylistic,
  pluginVue.configs['flat/recommended'],
  vueTsConfigs.recommended,
)
