### **Opis biznesowy projektu**

Projekt dotyczy stworzenia aplikacji do zarządzania użytkownikami oraz książkami, w tym wypożyczaniem i zwracaniem książek. Aplikacja ma na celu usprawnienie procesów związanych z obsługą użytkowników, zarządzaniem zasobami książek oraz monitorowaniem historii wypożyczeń. Dzięki temu biblioteki i inne instytucje mogą skuteczniej zarządzać swoimi zasobami oraz dostarczać użytkownikom lepsze doświadczenie.

Główne funkcje aplikacji:
1. **Zarządzanie użytkownikami** – rejestracja, logowanie.
2. **Zarządzanie książkami** – dodawanie, edycja, usuwanie oraz wyszukiwanie książek.
3. **Wypożyczanie i zwrot książek** – umożliwienie użytkownikom wypożyczania i zwracania książek oraz monitorowanie statusu książek.
4. **Monitorowanie historii wypożyczeń** – przechowywanie historii wypożyczeń, zarówno dla administratorów, jak i użytkowników.

---

### **Wymagania funkcjonalne**
1. **Moduł zarządzania użytkownikami:**
   - Rejestracja i logowanie użytkowników.
   - Generowanie tokenów JWT w celu uwierzytelniania.
   - Role użytkowników: Administrator i Zwykły Użytkownik.
   
2. **Moduł zarządzania książkami:**
   - Dodawanie nowych książek (dostępne dla administratorów).
   - Edycja szczegółów książek (dostępne dla administratorów).
   - Wyszukiwanie książek z możliwością sortowania i filtrowania .
   - Usuwanie książek przez administratora (dostępne dla administratorów).

3. **Moduł wypożyczania książek:**
   - Wypożyczanie książek przez użytkowników.
   - Sprawdzanie dostępności książek przed wypożyczeniem.
   - Zwrot książek przez użytkowników.
   - Historia wypożyczeń dostępna dla administratorów.

4. **Autoryzacja i uwierzytelnianie:**
   - Ograniczenie dostępu do niektórych funkcji na podstawie roli użytkownika.
   - Wymaganie uwierzytelnienia przed wykonaniem większości operacji.

---

### **Wymagania niefunkcjonalne**

1. **Bezpieczeństwo:**
   - Szyfrowanie haseł użytkowników.
   - Bezpieczne generowanie i przechowywanie tokenów JWT.
   - Ograniczenie dostępu do API w oparciu o role.

2. **Użyteczność:**
   - Intuicyjne i dobrze udokumentowane API.
   - Wyraźne komunikaty błędów dla użytkowników końcowych.

3. **Skalowalność:**
   - Możliwość rozbudowy o dodatkowe funkcjonalności w przyszłości.
   - Obsługa większej liczby użytkowników oraz książek bez istotnego wpływu na wydajność.

4. **Zgodność:**
   - Zgodność z REST API i najlepszymi praktykami dotyczącymi projektowania aplikacji webowych.
