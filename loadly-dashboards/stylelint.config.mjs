/** @type {import("stylelint").Config} */
export default {
  "extends": [
    "stylelint-config-standard",
    'stylelint-config-recommended-vue',
  ],
  rules: {
    "property-no-unknown": true,
    "unit-no-unknown": true,
    'no-empty-source': null,
    'function-no-unknown': [
      true, { ignoreFunctions: ['theme', 'v-bind'] },
    ],
    "at-rule-no-unknown": [true, {
      "ignoreAtRules": [
        "theme",
        "custom-variant",
        "source",
        "apply",
        "layer",
        "tailwind",
        "config",
        "import-theme",
        "/^--breakpoint-/"
      ],
    }],
    "custom-property-pattern": null
  },
  overrides: [
    {
      files: ['**/*.vue', '**/*.html'],
      customSyntax: 'postcss-html',
    },
    {
      files: ['**/*.css'],
      rules: {
        "property-no-unknown": true,
        "unit-no-unknown": true,
        "at-rule-no-unknown": [true, {
          "ignoreAtRules": [
            "theme",
            "custom-variant",
            "source",
            "apply",
            "layer",
            "tailwind",
            "config",
            "import-theme",
            "/^--breakpoint-/"
          ],
        }],
      },
    },
  ],
};
