#!/bin/bash

npx @openapitools/openapi-generator-cli generate -g aspnetcore \
  --additional-properties aspnetCoreVersion=8.0 \
  --additional-properties classModifier=abstract \
  --additional-properties operationModifier=abstract \
  --additional-properties packageName=projectManagement.API \
  --additional-properties packageTitle=projectManagement.API \
  --additional-properties enumValueSuffix= \
  --additional-properties operationResultTask=true \
  --additional-properties useSeparateModelProject=true \
  -i project.yml \
  -o .