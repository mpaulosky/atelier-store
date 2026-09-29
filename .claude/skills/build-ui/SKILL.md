---
name: build-ui
title: Build UI Skill
description: Build or update customer-facing ecommerce UI using the existing design system, components, product patterns and responsive conventions. Use when creating or significantly changing pages, sections or customer-facing components.
tags: [UI, frontend, design]
author: mpaulosky
version: 1.0.0
license: MIT
---

Build or update the following UI:

$ARGUMENTS

Before writing any code, call the Skill tool with "grilling" and interview me about this
UI request: layout and content, which existing components and patterns to reuse, states
(empty/loading/error/out-of-stock), responsive breakpoints, auth-dependent behavior, and
what "done" looks like. Look up facts in the codebase yourself rather than asking me.
Do not start step 1 until I confirm we've reached a shared understanding, then treat the
agreed decisions as the requirements for the steps below.

1. Inspect the current implementation and related components.
2. Search for reusable components and patterns that can be leveraged before creating new ones.
3. Reuse the existing design system and patterns.
4. Keep the change focused on the requested feature or component.
5. Verify responsive behavior.
6. Run and visually verify the application.
7. Run the normal project checks and tests to ensure nothing else is broken.
8. Fix reproducible issues without expanding the scope of the current change.
9. Summarize the changes made and check them against the decisions agreed during grilling.
10. Document any new components or patterns created for future reference.
