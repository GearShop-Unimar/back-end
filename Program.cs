name: SonarCloud Scan

on:
push:
branches:
-main
  pull_request:
types: [opened, synchronize, reopened]

jobs:
build:
name: Build and Analyze
    runs-on: windows - latest
    steps:
-name: Checkout code
        uses: actions / checkout@v4
        with:
          fetch - depth: 0

      - name: Cache SonarQube Cloud packages
        uses: actions / cache@v4
        with:
          path: ~\sonar\cache
          key: ${ { runner.os } }
-sonar
          restore - keys: ${ { runner.os } }
-sonar

      - name: Cache SonarQube Cloud scanner
        id: cache - sonar - scanner
        uses: actions / cache@v4
        with:
          path: ${ { runner.temp } }\scanner
          key: ${ { runner.os } }
-sonar - scanner
          restore - keys: ${ { runner.os } }
-sonar - scanner

      - name: Install SonarQube Cloud scanner
        if: steps.cache - sonar - scanner.outputs.cache - hit != 'true'
        shell: powershell
        run: |
          New - Item - Path ${ { runner.temp } }\scanner - ItemType Directory
          dotnet tool update dotnet-sonarscanner --tool-path ${{ runner.temp }}\scanner

      - name: Build and analyze
        env:
          SONAR_TOKEN: ${ { secrets.SONAR_TOKEN } }
shell: powershell
run: |
          ${ { runner.temp } }\scanner\dotnet-sonarscanner begin /k:"GearShop-Unimar_back-end" / o:"gearshop-unimar" / d:sonar.token = "${{ secrets.SONAR_TOKEN }}"
          dotnet build
          ${{ runner.temp }}\scanner\dotnet-sonarscanner end /d:sonar.token = "${{ secrets.SONAR_TOKEN }}"