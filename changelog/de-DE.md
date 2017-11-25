# Versionshinweis

## Version 1.9.0.0

### Live-Tile
Diese App unterstützt nun die Windows 10 Live-Tiles.  
Das heißt, wenn man die App auf dem Start-Screen angepinnt hat, kann man direkt auf der Kachel einige Informationen aus c-time sehen.  
Zum Beispiel wie lange man heute schon gearbeitet hat, oder welche Zeiten man heute schon erfasst hat.

### Benachrichtigung wenn jemand einstempelt
Das ist gerade morgens oder nach der Mittagspause hilfreich.  
Wenn man auf einen Arbeitskollegen wartet bekommt man hiermit nun einfach eine Benachrichtigung wenn dieser eingestempelt hat.

### Verbesserte kommunikation mit c-time
Beim Aufrufen der c-time API gibt es zwei kleine Verbesserungen.  
Zum Einen wurde ein kleine Cache eingebaut. Wenn man also schnell in der App hin- und herwechselt, ist die App wesentlich performanter.  
Zum Anderen wird bei einem Fehler die c-time API erneut angesprochen. Das hilft gerade bei schlechter Verbindung.

### Fehler in Statistiken behoben
In den Statistiken kam es zu einem Fehler, wenn man in der ausgewählten Zeitspanne bisher keine Zeiten hatte.  
Dieser Fehler wurde behoben.

### Verbesserte "Meine Zeiten"
Unter "Meine Zeiten" sieht man jetzt auch was für eine Zeit das war, Urlaub oder Home-Office zum Beispiel.  
Außerdem bekommt man eine Warnung wenn bei an einem Arbeitstag keine Zeiten eingetragen sind.

## Version 1.8.2.0

### Fehler mit heutigem Arbeitsende
In der letzten Version hat sich ein Fehler eingeschlichten, wodurch das heutige Arbeitsende nicht richtig funktioniert hat.  
Das ist in dieser Version wieder behoben.  

## Version 1.8.1.0

### Pausendauer in der Übersicht
In der Übersicht gibt es nun, ähnlich wie für die Überstunden, auch für die aktuelle Pausendauer eine Uhr.  
Man sieht nun auf einen Blick wie lang die aktuelle Pause schon geht, und wann diese vorbei ist.  

### Anwesenheitsliste funktioniert wieder
Leider kam es mit der letzten Version zu einem Fehler.  
Dadurch wurde in der Anwesenheitsliste jeder als Abwesend angezeigt.  
Das ist jetzt behoben. Die Anwesenheitsliste sollte jetzt wieder wie gewohnt funktionieren.  

### Neue Einstellung für Arbeitstage
Es gibt jetzt eine neue Einstellung mit der man angeben kann, welche Tage der Woche die Arbeitstage sind.  
Dies wird zum Beispiel bei der Überstunden-Statistik berücksichtigt.  

## Version 1.8.0.0

### Anwesenheitsliste erweitert
Die Anwesenheitsliste zeigt nun an, wie jemand ein- oder ausgestempelt ist.  
Ist also jemand über Home-Office eingestempelt, kann man das nun auch erkennen.  
Außerdem gibt es jetzt eine Suchfunktion in der Anwesenheitsliste.  

### Über und Einstellungen überarbeitet
Der Über Bereich wurde auch komplett überarbeitet.  
Dort gibt es jetzt auch die Möglichkeit Feedback über den Feedback Hub zu geben.  
In den Einstellungen gibt es eine neue Einstellung zur Analytik.  

### Neue Diagramme
Die Diagramme bei den Statistiken wurden komplett überarbeitet.  
Sie sehen jetzt wesentlich moderner aus und haben auch einige neue Funktionen.

## Version 1.7.1.0

### Absturz beim Starten
Auf einigen Computern ist diese App beim Start direkt abgestürzt. Das sollte jetzt behoben sein.  

### Updatehinweis erneut ansehen
Im Bereich "Über" kann man sich diese Updatehinweise erneut anzeigen lassen.

## Version 1.7.0.0

### Eigene Mitarbeitergruppen!
Um die Übersicht zu verbessern, kann man in der Anwesenheitsliste nun eigene Gruppen von Mitarbeitern erstellen.  

### Start- und Enddatum
Im Bereich der Zeiten und in den Statistiken wird sich das Start- und Enddatum jetzt gemerkt.  

### Teilen!
Einige Infos aus dieser App kann man jetzt mit anderen Apps teilen.  
Dadurch kann man zum Beispiel seine Zeiten per E-Mail versenden.

## Version 1.6.1.0

### Erweiterte Anwesenheitsliste!
In der Anwesenheitsliste kann man nun einen Mitarbeiter anklicken um mehr Infos von diesem zu sehen.  
Dort kann man ihm zum Beispiel direkt eine E-Mail senden, direkt anrufen, oder den eigenen Kontakten hinzufügen.  

### Allgemeine Verbesserungen!
Direkt nach dem ersten Einstempeln konnte es vorkommen, dass die Stoppuhr rückwärts lief - jetzt nicht mehr.  
Unter "Meine Zeiten" sieht man jetzt, ähnlich wie in den Statistiken, im Titel den ausgewählten Zeitraum.  
Überall in der ganzen App wurde die Akzentfarbe eingebaut.  

### Verbesserungen bei den Statistiken!
Es gibt eine neue Statistik für das heutige Arbeitsende ohne Überstunden.  
Außerdem wurde die Positionierung gerade bei langen Statistiknamen angepasst.  
In der letzten Version hat sich ein kleiner Fehler eingeschlichen, weshalb die Buttons für "Diesen Monat", "Letzten Monat" und "Letzte 7 Tage" nicht funktioniert haben. Das ist auch behoben.  

## Version 1.6.0.0

### Geolokalisierung beim Stempeln!
c-time hat diese Funktion schon länger unterstützt, jetzt unterstützt auch diese App die Geolokalisierung beim Stempeln.  
Dabei wird, wenn man ein- oder ausstempelt, die eigene Position an c-time übertragen.  
In der Übersicht wird dafür nun ein eigenes Icon angezeigt.  

### Verbesserungen in der Übersicht
Wenn man seine heutige Arbeitszeit voll hat, werden die Überstunden separat hochgezählt.  
Klick auf seine Zeit, wechselt man automatisch zu "Meine Zeiten" für den heutigen Tag.  

### Update auf die neue c-time API!
Diese App verwendet nun die neue c-time API.  
Dadurch ist die Anmeldung sicherer geworden, das Passwort wird jetzt nicht mehr an den Server übertragen.  
Außerdem wird die Anwesenheitsliste nun viel schneller geladen.  

### Einstellungen überarbeitet!
In den neuen Einstellungen kann man hinterlegen wie lange ein Arbeitstag und die Pause dauert.  
Diese Werte werden dann in den Statistiken und in der Übersicht für die Überstunden verwendet.  
Außerdem kann man das Theme dieser App einstellen.  

### Andere Verbesserungen!
In den Statistiken kann man jetzt selbst bestimmen, ob der heutige Tag mit in die Statistiken einbezogen werden soll oder nicht.