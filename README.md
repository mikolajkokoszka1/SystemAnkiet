# Dokumentacja Projektu Systemu Ankiet
### Mikołaj Kokoszka

## 1. Opis Projektu
System Ankiet to aplikacja internetowa stworzona w technologii ASP.NET Core MVC, służąca do tworzenia, zarządzania i wypełniania ankiet. Projekt został zaprojektowany zgodnie z wzorcem Model-View-Controller.
Aplikacja umożliwia pracę w trzech rolach: administratora, zalogowanego i niezalogowanego użytkownika.


## 2. Instalacja i Wymagania

### Wymagania systemowe
- **Środowisko uruchomieniowe**: .NET 8.0 SDK
- **Baza danych**: Microsoft SQL Server (LocalDB) - instalowany razem z Visual Studio
- **IDE**: Visual Studio 2022 z workloader ASP.NET and web development

### Instrukcja Instalacji
1. Pobierz kod źródłowy projektu (Download ZIP).
2. Rozpakuj archiwum i otwórz plik rozwiązania `WebApplication1.slnx` w Visual Studio.
3. Upewnij się, że masz zainstalowany serwer SQL Server LocalDB (jest on częścią instalacji Visual Studio z workloadem ASP.NET).
4. Otwórz konsolę Menedżera Pakietów (Package Manager Console) i wykonaj polecenie aktualizacji bazy danych, aby utworzyć schemat i tabele:
   ```powershell
   Update-Database
   ```
5. Uruchom aplikację (F5).


## 3. Konfiguracja

### Połączenie z Bazą Danych
Aplikacja domyślnie skonfigurowana jest do użycia lokalnej bazy danych LocalDB. Łańcuch połączenia znajduje się w pliku `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AnkietyDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Użytkownicy
Podczas pierwszego uruchomienia, aplikacja automatycznie tworzy konta testowe z przypisanymi rolami.

| Rola | Email (Login) | Hasło | Uprawnienia |
|---|---|---|---|
| **Administrator** | `admin@ankiety.pl` | `Admin123!` | Pełny dostęp: Tworzenie, edycja, usuwanie ankiet i pytań. |
| **Użytkownik** | `user@ankiety.pl` | `User123!` | Dostęp podstawowy: Przeglądanie i wypełnianie ankiet. |


## 4. Instrukcja Użytkownika

### Strona Główna i Dostęp
- Aplikacja posiada stronę startową dostępną publicznie.
- Aby uzyskać dostęp do niektórych funkcji systemu, należy się zarejestrować lub zalogować.
- Panel logowania/rejestracji dostępny jest w prawym górnym rogu ekranu.

### Uprawnienia administratora
   - Dostęp do listy ankiet.
   - Możliwość oddania głosu
   - Możliwość utworzenia nowej ankiety.
   - Możliwość edytowania ankiety, pytań oraz odpowiedzi.
   - Możliwość usuwania ankiety
   - Możliwość sprawdzenia wyników ankiety
   - Możliwość wejścia w szczegóły ankiety

### Uprawnienia zalogowanego użytkownika
  - Dostęp do listy ankiet.
  - Możliwość oddania głosu
  - Możliwość sprawdzenia wyników ankiety
  - Możliwość wejścia w szczegóły ankiety

### Uprawnienia niezalogowanego użytkownika
- Dostęp do listy ankiet.
- Możliwość wejścia w szczegóły ankiety

Aplikacja pozwala na tworzenie ankiet z pytaniami jednokrotnego i wielokrotnego wyboru. Po zalogowaniu użytkownika należy wejść w opcję "Lista" w menu by przejść do listy dostępnych ankiet. Będąc administratorem, uzyskamy tam dostęp do kreatora ankiet. Możemu z tamtego miejsca również nimi zarządzać, podglądać wyniki, czy całkowicie usuwać. Użytkownik może wchodzić w interakcje z gotowymi ankietami, ale w ograniczonym zakresie (wypełnianie i podglądanie wyników, bez edycji).




## 5. Encje
W projekcie zdefiniowano 5 encji:

1. Survey - encja główna- Przechowuje podstawowe informacje o ankiecie.

- Survey 1:N Question - jedna ankieta może mieć wiele pytań
- Survey 1:N Response - jedna ankieta może mieć wiele wypełnionych odpowiedzi

2. Question - Reprezentuje pytanie w ankiecie.

- Question N:1 Survey - wiele pytań należy do jednej ankiety
- Question 1:N Answer - jedno pytanie może mieć wiele opcji odpowiedzi

3. Answer - Przechowuje możliwe odpowiedzi do pytania zamkniętego.

- Answer N:1 Question - wiele odpowiedzi należy do jednego pytania
- Answer 1:N ResponseDetail - jedna opcja może być wybrana w wielu odpowiedziach użytkowników

4. Response - Reprezentuje pojedyncze wypełnienie ankiety przez użytkownika.

- Response N:1 Survey - wiele odpowiedzi należy do jednej ankiety
- Response 1:N ResponseDetail - jedna wypełniona ankieta zawiera wiele szczegółowych odpowiedz

5. ResponseDetail - Przechowuje konkretne zaznaczone opcje przez użytkownika.

- ResponseDetail N:1 Response - wiele szczegółów należy do jednej wypełnionej ankiety
- ResponseDetail N:1 Answer - wiele szczegółów może wskazywać na tę samą opcję odpowiedzi

## 6. Walidacja Formularzy
Aplikacja zawiera formularze z walidacją danych (Data Annotations), m.in.:
- **Tworzenie Ankiety**: Wymagany tytuł, ograniczenie długości opisu.
- **Tworzenie Pytania**: Wymagana treść pytania, typ oraz kolejność (liczba porządkowa według której pytania będą układane na liście).
- **Tworzenie Odpowiedzi**: Wymagana treść odpowiedzi oraz kolejność (tak jak powyżej, jest to numer porządkowy).
- **Logowanie/Rejestracja**: Walidacja formatu email i złożoności hasła.

## 7. API
Zaimplementowano API CRUD umożliwiające operacje na głównej encji ankiet.
Endpoint: `/api/SurveyApi`

- `GET /api/SurveyApi` - Pobierz wszystkie ankiety
- `GET /api/SurveyApi/{id}` - Pobierz ankietę po ID
- `POST /api/SurveyApi` - Utwórz nową ankietę
- `PUT /api/SurveyApi/{id}` - Edytuj ankietę
- `DELETE /api/SurveyApi/{id}` - Usuń ankietę
