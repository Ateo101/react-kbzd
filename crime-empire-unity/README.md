# Crime Empire (Unity, Android)

Мобильная игра про криминальную империю: idle arcade база + автобой + 4X-карта города.
Референс — Crime Empire: Gang War. Полный разбор и план — в [docs/GDD.md](docs/GDD.md).

## Что здесь есть

Каркас кода для основного цикла, без сцен и арта:

- кошелёк с лимитами и форматом `15.5k`, площадки с постепенной оплатой `4600/5000`;
- бизнесы с очередью клиентов на NavMesh, стопка денег на столе, менеджеры-боссы;
- найм боссов, их уровни и характеристики (ScriptableObject);
- рейды полиции волнами, автострельба, полоски здоровья;
- карта районов: походы, бой, ИИ-банды, доход в час;
- оффлайн-доход, сохранение JSON с резервной копией;
- 17 EditMode-тестов на экономику и карту.

## Как открыть

1. Unity Hub → Add → Add project from disk → эта папка.
2. Версия: Unity 6 LTS (6000.0.x). Если стоит другая 6000.x — Hub предложит обновить, соглашайтесь.
3. File → Build Profiles → Android → Switch Platform.
4. Player Settings: IL2CPP, ARM64, minSdk 24, формат AAB.
5. Window → General → Test Runner → EditMode → Run All.

Сборка первой сцены по шагам — в разделе 7 GDD.

## Структура

```
Assets/_Project/Scripts   код (сборка CrimeEmpire.asmdef)
Assets/_Project/Tests     EditMode-тесты
docs/GDD.md               концепт, архитектура, баланс, план
```

## Перенос в отдельный репозиторий

Проект лежит папкой внутри другого репо, потому что создать новый репо отсюда не получилось.
Создайте пустой репозиторий на GitHub (например `crime-empire-unity`), потом:

```bash
git clone --branch ccr-643e6d1c-e2cgl5 https://github.com/ateo101/react-kbzd.git tmp
cd tmp
git subtree split --prefix=crime-empire-unity -b crime-empire
git push https://github.com/<you>/crime-empire-unity.git crime-empire:main
```

История коммитов этой папки сохранится.
