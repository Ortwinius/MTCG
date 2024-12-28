@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo Syntax: $1 [pause]
echo - pause: optional, if set, the script will pause after each block
echo.

set "pauseFlag=0"
for %%a in (%*) do (
    if /I "%%a"=="pause" (
        set "pauseFlag=1"
    )
)

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"admin\",    \"Password\":\"istrator\"}"
echo "Should return HTTP 201"
echo.

REM if %pauseFlag%==1 pause

echo should fail:
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo "Should return HTTP 4xx - User already exists"
echo.
curl -i -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo "Should return HTTP 4xx - User already exists"
echo. 
echo.

REM if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 2) Login Users
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo "should return HTTP 200 with generated token for the user, here: kienboec-mtcgToken"
echo.
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo "should return HTTP 200 with generated token for the user, here: altenhof-mtcgToken"
echo.
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"admin\",    \"Password\":\"istrator\"}"
echo "should return HTTP 200 with generated token for the user, here: admin-mtcgToken"
echo.

REM if %pauseFlag%==1 pause

echo should fail:
curl -i -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo "Should return HTTP 4xx - Login failed"
echo.
echo.

if %pauseFlag%==1 pause

REM ---

REM --------------------------------------------------
echo 3) create packages (done by "admin")
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 10, \"MonType\":\"Goblin\"}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 50, \"MonType\":\"Dragon\"}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 20, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 45, \"MonType\":\"Ork\"}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\", \"Type\":\"SpellCard\", \"Element\":\"Fire\", \"Damage\": 25, \"SpellType\":\"FireSpell\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"644808c2-f87a-4600-b313-122b02322fd5\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 9, \"MonType\":\"Goblin\"}, {\"Id\":\"4a2757d6-b1c3-47ac-b9a3-91deab093531\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 55, \"MonType\":\"Dragon\"}, {\"Id\":\"91a6471b-1426-43f6-ad65-6fc473e16f9f\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 21, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"4ec8b269-0dfa-4f97-809a-2c63fe2a0025\", \"Name\":\"Ork\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 55, \"MonType\":\"Ork\"}, {\"Id\":\"f8043c23-1534-4487-b66b-238e0c3c39b5\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 23, \"SpellType\":\"WaterSpell\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"b017ee50-1c14-44e2-bfd6-2c0c5653a37c\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 11, \"MonType\":\"Goblin\"}, {\"Id\":\"d04b736a-e874-4137-b191-638e0ff3b4e7\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 70, \"MonType\":\"Dragon\"}, {\"Id\":\"88221cfe-1f84-41b9-8152-8e36c6a354de\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 22, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"1d3f175b-c067-4359-989d-96562bfa382c\", \"Name\":\"Ork\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 40, \"MonType\":\"Ork\"}, {\"Id\":\"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\", \"Name\":\"RegularSpell\", \"Type\":\"SpellCard\", \"Element\":\"Normal\", \"Damage\": 28, \"SpellType\":\"RegularSpell\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"ed1dc1bc-f0aa-4a0c-8d43-1402189b33c8\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 10, \"MonType\":\"Goblin\"}, {\"Id\":\"65ff5f23-1e70-4b79-b3bd-f6eb679dd3b5\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 50, \"MonType\":\"Dragon\"}, {\"Id\":\"55ef46c4-016c-4168-bc43-6b9b1e86414f\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 20, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"f3fad0f2-a1af-45df-b80d-2e48825773d9\", \"Name\":\"Ork\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 45, \"MonType\":\"Ork\"}, {\"Id\":\"8c20639d-6400-4534-bd0f-ae563f11f57a\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 25, \"SpellType\":\"WaterSpell\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"d7d0cb94-2cbf-4f97-8ccf-9933dc5354b8\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 9, \"MonType\":\"Goblin\"}, {\"Id\":\"44c82fbc-ef6d-44ab-8c7a-9fb19a0e7c6e\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 55, \"MonType\":\"Dragon\"}, {\"Id\":\"2c98cd06-518b-464c-b911-8d787216cddd\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 21, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"951e886a-0fbf-425d-8df5-af2ee4830d85\", \"Name\":\"Ork\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 55, \"MonType\":\"Ork\"}, {\"Id\":\"dcd93250-25a7-4dca-85da-cad2789f7198\", \"Name\":\"FireSpell\", \"Type\":\"SpellCard\", \"Element\":\"Fire\", \"Damage\": 23, \"SpellType\":\"FireSpell\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"b2237eca-0271-43bd-87f6-b22f70d42ca4\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 11, \"MonType\":\"Goblin\"}, {\"Id\":\"9e8238a4-8a7a-487f-9f7d-a8c97899eb48\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 70, \"MonType\":\"Dragon\"}, {\"Id\":\"d60e23cf-2238-4d49-844f-c7589ee5342e\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 22, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"fc305a7a-36f7-4d30-ad27-462ca0445649\", \"Name\":\"Ork\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 40, \"MonType\":\"Ork\"}, {\"Id\":\"84d276ee-21ec-4171-a509-c1b88162831c\", \"Name\":\"RegularSpell\", \"Type\":\"SpellCard\", \"Element\":\"Normal\", \"Damage\": 28, \"SpellType\":\"RegularSpell\"}]"
echo "Should return HTTP 201"
echo.
echo.

REM if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 4) Acquire Packages kienboec
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d ""
echo "Should return HTTP 201"
echo.

curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d ""
echo "Should return HTTP 201"
echo.

REM if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 5) Acquire Packages altenhof
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d ""
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d ""
echo "Should return HTTP 201"
echo.

REM --------------------------------------------------
REM add new packages is next
REM --------------------------------------------------

REM THIS PART IS WRONG --------------------------------------------------
REM --------------------------------------------------
if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 6) add new packages
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 10, \"MonType\":\"Goblin\"}, {\"Id\":\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"Name\":\"RegularSpell\", \"Type\":\"SpellCard\", \"Element\":\"Normal\", \"Damage\": 50, \"SpellType\":\"RegularSpell\"}, {\"Id\":\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"Name\":\"Knight\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 20, \"MonType\":\"Knight\"}, {\"Id\":\"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\", \"Name\":\"RegularSpell\", \"Type\":\"SpellCard\", \"Element\":\"Normal\", \"Damage\": 45, \"SpellType\":\"RegularSpell\"}, {\"Id\":\"2508bf5c-20d7-43b4-8c77-bc677decadef\", \"Name\":\"FireElf\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 25, \"MonType\":\"FireElf\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"70962948-2bf7-44a9-9ded-8c68eeac7793\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 9, \"MonType\":\"Goblin\"}, {\"Id\":\"74635fae-8ad3-4295-9139-320ab89c2844\", \"Name\":\"FireSpell\", \"Type\":\"SpellCard\", \"Element\":\"Fire\", \"Damage\": 55, \"SpellType\":\"FireSpell\"}, {\"Id\":\"ce6bcaee-47e1-4011-a49e-5a4d7d4245f3\", \"Name\":\"Knight\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 21, \"MonType\":\"Knight\"}, {\"Id\":\"a6fde738-c65a-4b10-b400-6fef0fdb28ba\", \"Name\":\"FireSpell\", \"Type\":\"SpellCard\", \"Element\":\"Fire\", \"Damage\": 55, \"SpellType\":\"FireSpell\"}, {\"Id\":\"a1618f1e-4f4c-4e09-9647-87e16f1edd2d\", \"Name\":\"FireElf\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 23, \"MonType\":\"FireElf\"}]"
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"2272ba48-6662-404d-a9a1-41a9bed316d9\", \"Name\":\"WaterGoblin\", \"Type\":\"MonsterCard\", \"Element\":\"Water\", \"Damage\": 11, \"MonType\":\"Goblin\"}, {\"Id\":\"3871d45b-b630-4a0d-8bc6-a5fc56b6a043\", \"Name\":\"Dragon\", \"Type\":\"MonsterCard\", \"Element\":\"Fire\", \"Damage\": 70, \"MonType\":\"Dragon\"}, {\"Id\":\"166c1fd5-4dcb-41a8-91cb-f45dcd57cef3\", \"Name\":\"Knight\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 22, \"MonType\":\"Knight\"}, {\"Id\":\"237dbaef-49e3-4c23-b64b-abf5c087b276\", \"Name\":\"WaterSpell\", \"Type\":\"SpellCard\", \"Element\":\"Water\", \"Damage\": 40, \"SpellType\":\"WaterSpell\"}, {\"Id\":\"27051a20-8580-43ff-a473-e986b52f297a\", \"Name\":\"RegularElf\", \"Type\":\"MonsterCard\", \"Element\":\"Normal\", \"Damage\": 28, \"MonType\":\"RegularElf\"}]"
echo "Should return HTTP 201"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 7) acquire newly created packages altenhof
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d ""
echo "Should return HTTP 201"
echo.
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d ""
echo "Should return HTTP 201"
echo.
echo should fail (no money):
curl -i -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d ""
echo "Should return HTTP 4xx - Not enough money"
echo.
echo.


if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 8) show all acquired cards kienboec
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo should fail (no token)
curl -i -X GET http://localhost:10001/cards 
echo "Should return HTTP 4xx - Unauthorized"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 9) show all acquired cards altenhof
curl -i -X GET http://localhost:10001/cards --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 10) show unconfigured deck
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and a empty-list"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and a empty-list"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 11) configure deck
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\"]"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "[\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"d60e23cf-2238-4d49-844f-c7589ee5342e\", \"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\"]"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.

if %pauseFlag%==1 pause
echo should fail and show original from before:
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\"]"
echo "Should return HTTP 4xx"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.
echo should fail ... only 3 cards set
curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "[\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"d60e23cf-2238-4d49-844f-c7589ee5342e\"]"
echo "Should return HTTP 4xx - Bad request"
echo.

if %pauseFlag%==1 pause


REM --------------------------------------------------
echo 12) show configured deck 
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
curl -i -X GET http://localhost:10001/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 13) show configured deck different representation
echo kienboec
curl -i -X GET "http://localhost:10001/deck?format=plain" --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.
echo altenhof
curl -i -X GET "http://localhost:10001/deck?format=plain" --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and a list of all cards"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 14) edit user data
echo.
curl -i -X GET http://localhost:10001/users/kienboec --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and current user data"
echo.
curl -i -X GET http://localhost:10001/users/altenhof --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and current user data"
echo.
curl -i -X PUT http://localhost:10001/users/kienboec --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Name\": \"Kienboeck\",  \"Bio\": \"me playin...\", \"Image\": \":-)\"}"
echo "Should return HTTP 2xx"
echo.
curl -i -X PUT http://localhost:10001/users/altenhof --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Name\": \"Altenhofer\", \"Bio\": \"me codin...\",  \"Image\": \":-D\"}"
echo "Should return HTTP 2xx"
echo.
curl -i -X GET http://localhost:10001/users/kienboec --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and new user data"
echo.
curl -i -X GET http://localhost:10001/users/altenhof --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and new user data"
echo.
echo.

if %pauseFlag%==1 pause

REM TODO 
REM NEED TO UPDATE USERT OBJ IN DB

echo should fail:
curl -i -X GET http://localhost:10001/users/altenhof --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 4xx"
echo.
curl -i -X GET http://localhost:10001/users/kienboec --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 4xx"
echo.
curl -i -X PUT http://localhost:10001/users/kienboec --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Name\": \"Hoax\",  \"Bio\": \"me playin...\", \"Image\": \":-)\"}"
echo "Should return HTTP 4xx"
echo.
curl -i -X PUT http://localhost:10001/users/altenhof --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Name\": \"Hoax\", \"Bio\": \"me codin...\",  \"Image\": \":-D\"}"
echo "Should return HTTP 4xx"
echo.
curl -i -X GET http://localhost:10001/users/someGuy  --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 4xx"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 15) stats
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and user stats"
echo.
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and user stats"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 16) scoreboard
curl -i -X GET http://localhost:10001/scoreboard --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and the scoreboard"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 17) battle
start /b "kienboec battle" curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer kienboec-mtcgToken"
start /b "altenhof battle" curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer altenhof-mtcgToken"
ping localhost -n 10 >NUL 2>NUL

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 18) Stats 
echo kienboec
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and changed user stats"
echo.
echo altenhof
curl -i -X GET http://localhost:10001/stats --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and changed user stats"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 19) scoreboard
curl -i -X GET http://localhost:10001/scoreboard --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and the changed scoreboard"
echo.
echo.

if %pauseFlag%==1 pause









REM --------------------------------------------------
REM ITS WRONG FROM HERE ON OUT
REM NEED TO UPDATE THE CARD OBJECTS
REM --------------------------------------------------








echo 20) trade
echo check trading deals
curl -i -X GET http://localhost:10001/tradings --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and an empty list"
echo.
echo create trading deal
curl -i -X POST http://localhost:10001/tradings --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}"
echo "Should return HTTP 201"
echo.

if %pauseFlag%==1 pause

echo check trading deals
curl -i -X GET http://localhost:10001/tradings --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 - and the trading deal"
echo.
curl -i -X GET http://localhost:10001/tradings --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 - and the trading deal"
echo.

if %pauseFlag%==1 pause

echo delete trading deals
curl -i -X DELETE http://localhost:10001/tradings/6cd85277-4590-49d4-b0cf-ba0a921faad0 --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 2xx"
echo.
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 21) check trading deals
curl -i -X GET http://localhost:10001/tradings  --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 ..."
echo.
curl -i -X POST http://localhost:10001/tradings --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}"
echo "Should return HTTP 201"
echo check trading deals
curl -i -X GET http://localhost:10001/tradings  --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 ..."
echo.
curl -i -X GET http://localhost:10001/tradings  --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 ..."
echo.

if %pauseFlag%==1 pause

echo try to trade with yourself (should fail)
curl -i -X POST http://localhost:10001/tradings/6cd85277-4590-49d4-b0cf-ba0a921faad0 --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "\"4ec8b269-0dfa-4f97-809a-2c63fe2a0025\""
echo "Should return HTTP 4xx"
echo.

if %pauseFlag%==1 pause

echo try to trade 
echo.
curl -i -X POST http://localhost:10001/tradings/6cd85277-4590-49d4-b0cf-ba0a921faad0 --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "\"951e886a-0fbf-425d-8df5-af2ee4830d85\""
echo "Should return HTTP 201 ..."
echo.
curl -i -X GET http://localhost:10001/tradings --header "Authorization: Bearer kienboec-mtcgToken"
echo "Should return HTTP 200 ..."
echo.
curl -i -X GET http://localhost:10001/tradings --header "Authorization: Bearer altenhof-mtcgToken"
echo "Should return HTTP 200 ..."
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL
@echo on
