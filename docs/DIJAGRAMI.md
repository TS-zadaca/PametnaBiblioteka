# Pareto dijagram i histogram nakon inspekcije

Izvor su stvarni nalazi u `nalazi.csv`. Jedan red = jedan jedinstven nalaz. U `vrsta` upišite `greska` ili `sugestija`. Odvojite sugestije od potvrđenih grešaka; jasno napišite koji skup prikazujete.

## Pareto u Excelu

1. Uvezite CSV i filtrirajte `vrsta = greska`.
2. Prebrojite greške po kategoriji (npr. Validacija, Logika, Podaci, Interfejs, Održivost) kroz pivot tabelu.
3. Poredajte kategorije od najvećeg broja prema najmanjem.
4. Dodajte kumulativni postotak: tekući zbir / ukupan broj grešaka.
5. Napravite kombinovani grafikon: stubovi su brojevi, linija je kumulativni postotak na desnoj osi 0–100%.
6. Naslov, nazivi osa i veličina skupa moraju biti navedeni. Kod nula grešaka nemojte dijeliti nulom: napišite da Pareto nije smislen bez nalaza.

## Histogram

Histogram traži numeričku varijablu, ne samo nazive kategorija. Za svaki zatvoreni nalaz zabilježite stvarno vrijeme otklanjanja u minutama u pomoćnoj tabeli (ID nalaza, minute). Ne nagađajte vremena.

1. Odredite jednake intervale, npr. [0,10), [10,20), [20,30), [30,40), uz proširenje do najvećeg izmjerenog vremena.
2. Izbrojite nalaze u svakom intervalu.
3. Napravite histogram s dodirujućim stubovima; X = minute otklanjanja, Y = broj nalaza.
4. Navedite broj mjerenja i pravilo intervala; nezatvorene nalaze bez trajanja isključite i napišite koliko ih je.

Ako asistent na vježbama propisuje drugu varijablu histograma, primijenite njegovo pravilo. Za cijeli tim koristite podatke sva tri modula, uz dodatnu kolonu `modul`. Nema unaprijed nacrtanih dijagrama s izmišljenim podacima.
