using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using HonkAPI.Core;

namespace HonkAPI.UI
{
    public class HonkModsMenuScreen : MonoBehaviour, IMenuScreen, IMenuMouseable
    {
        public static HonkModsMenuScreen Instance { get; private set; }

        public LittleMenu littleMenu;
        public GameObject backHighlight;
        public GameObject backBtnObj;
        public TMP_Text backBtnTMP;
        public TMP_Text contentTMP;
        private bool initialized;
        private int scrollOffset = 0;
        private const int MaxVisibleMods = 3;

        public static string GetFolderText(LocalizationLanguage lang)
        {
            switch (lang)
            {
                case LocalizationLanguage.french:
                    return "Dossier : mods/ ou plugins/";
                case LocalizationLanguage.italian:
                    return "Cartella: mods/ o plugins/";
                case LocalizationLanguage.german:
                    return "Ordner: mods/ oder plugins/";
                case LocalizationLanguage.spanish:
                    return "Carpeta: mods/ o plugins/";
                case LocalizationLanguage.simplifiedChinese:
                    return "文件夹：mods/ 或 plugins/";
                case LocalizationLanguage.japanese:
                    return "フォルダー: mods/ または plugins/";
                case LocalizationLanguage.russian:
                    return "Папка: mods/ или plugins/";
                case LocalizationLanguage.korean:
                    return "폴더: mods/ 또는 plugins/";
                case LocalizationLanguage.polish:
                    return "Folder: mods/ lub plugins/";
                case LocalizationLanguage.turkish:
                    return "Klasör: mods/ veya plugins/";
                case LocalizationLanguage.czech:
                    return "Složka: mods/ nebo plugins/";
                case LocalizationLanguage.dutch:
                    return "Map: mods/ of plugins/";
                case LocalizationLanguage.brazilianPortuguese:
                    return "Pasta: mods/ ou plugins/";
                case LocalizationLanguage.english:
                default:
                    return "Folder: mods/ or plugins/";
            }
        }

        public static string GetNoModsText(LocalizationLanguage lang)
        {
            switch (lang)
            {
                case LocalizationLanguage.french:
                    return "Aucun mod actif.";
                case LocalizationLanguage.italian:
                    return "Nessun mod attivo.";
                case LocalizationLanguage.german:
                    return "Keine aktiven Mods.";
                case LocalizationLanguage.spanish:
                    return "Ningún mod activo.";
                case LocalizationLanguage.simplifiedChinese:
                    return "无已启用的模组。";
                case LocalizationLanguage.japanese:
                    return "アクティブなModはありません。";
                case LocalizationLanguage.russian:
                    return "Нет активных модов.";
                case LocalizationLanguage.korean:
                    return "활성화된 모드가 없습니다.";
                case LocalizationLanguage.polish:
                    return "Brak aktywnych modów.";
                case LocalizationLanguage.turkish:
                    return "Aktif mod yok.";
                case LocalizationLanguage.czech:
                    return "Žádné aktivní módy.";
                case LocalizationLanguage.dutch:
                    return "Geen actieve mods.";
                case LocalizationLanguage.brazilianPortuguese:
                    return "Nenhum mod ativo.";
                case LocalizationLanguage.english:
                default:
                    return "No active mods.";
            }
        }

        public static string GetBackText(LocalizationLanguage lang)
        {
            switch (lang)
            {
                case LocalizationLanguage.french:
                    return "Retour";
                case LocalizationLanguage.italian:
                    return "Indietro";
                case LocalizationLanguage.german:
                    return "Zurück";
                case LocalizationLanguage.spanish:
                    return "Volver";
                case LocalizationLanguage.simplifiedChinese:
                    return "返回";
                case LocalizationLanguage.japanese:
                    return "戻る";
                case LocalizationLanguage.russian:
                    return "Назад";
                case LocalizationLanguage.korean:
                    return "뒤로";
                case LocalizationLanguage.polish:
                    return "Wstecz";
                case LocalizationLanguage.turkish:
                    return "Geri";
                case LocalizationLanguage.czech:
                    return "Zpět";
                case LocalizationLanguage.dutch:
                    return "Terug";
                case LocalizationLanguage.brazilianPortuguese:
                    return "Voltar";
                case LocalizationLanguage.english:
                default:
                    return "Back";
            }
        }

        public static string GetPaginationText(LocalizationLanguage lang, int start, int end, int total)
        {
            switch (lang)
            {
                case LocalizationLanguage.french:
                    return string.Format("({0}-{1} sur {2} - molette / flèches)", start, end, total);
                case LocalizationLanguage.italian:
                    return string.Format("({0}-{1} di {2} - scroll / frecce)", start, end, total);
                case LocalizationLanguage.german:
                    return string.Format("({0}-{1} von {2} - Mausrad / Pfeile)", start, end, total);
                case LocalizationLanguage.spanish:
                    return string.Format("({0}-{1} de {2} - scroll / flechas)", start, end, total);
                case LocalizationLanguage.simplifiedChinese:
                    return string.Format("({0}-{1} / {2} - 滚轮 / 方向键)", start, end, total);
                case LocalizationLanguage.japanese:
                    return string.Format("({0}-{1} / {2} - スクロール / 矢印)", start, end, total);
                case LocalizationLanguage.russian:
                    return string.Format("({0}-{1} из {2} - колёсико / стрелки)", start, end, total);
                case LocalizationLanguage.korean:
                    return string.Format("({0}-{1} / {2} - 스크롤 / 화살표)", start, end, total);
                case LocalizationLanguage.polish:
                    return string.Format("({0}-{1} z {2} - kółko / strzałki)", start, end, total);
                case LocalizationLanguage.turkish:
                    return string.Format("({0}-{1} / {2} - kaydırma / oklar)", start, end, total);
                case LocalizationLanguage.czech:
                    return string.Format("({0}-{1} z {2} - kolečko / šipky)", start, end, total);
                case LocalizationLanguage.dutch:
                    return string.Format("({0}-{1} van {2} - scroll / pijltjes)", start, end, total);
                case LocalizationLanguage.brazilianPortuguese:
                    return string.Format("({0}-{1} de {2} - scroll / setas)", start, end, total);
                case LocalizationLanguage.english:
                default:
                    return string.Format("({0}-{1} of {2} - scroll / arrows)", start, end, total);
            }
        }

        public LittleMenu mouseableLittleMenu
        {
            get { return littleMenu; }
        }

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            if (littleMenu == null)
            {
                littleMenu = new LittleMenu();
            }
            else
            {
                littleMenu.menuCommands.Clear();
                littleMenu.highlightElements.Clear();
                if (littleMenu.optionsEnabledState != null)
                {
                    littleMenu.optionsEnabledState.Clear();
                }
            }

            littleMenu.menuCommands.Add(GoToMainMenu);
            if (backHighlight != null)
            {
                littleMenu.highlightElements.Add(backHighlight);
            }
            if (littleMenu.optionsEnabledState != null)
            {
                littleMenu.optionsEnabledState.Add(true);
            }

            littleMenu.SetCurrentSelection(0, silently: true);
            initialized = true;
        }

        public void Activate()
        {
            if (!initialized)
            {
                Initialize();
            }

            scrollOffset = 0;
            RefreshModListText();

            gameObject.SetActive(true);
            base.enabled = true;
            UIInputManager.Subscribe(this);
            if (MainMenuBase.instance != null)
            {
                MainMenuBase.instance.currentScreen = this;
            }
            if (littleMenu != null)
            {
                littleMenu.SetCurrentSelection(0, silently: false);
            }
        }

        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0.05f)
            {
                ScrollUp();
            }
            else if (scroll < -0.05f)
            {
                ScrollDown();
            }
        }

        public void ScrollUp()
        {
            IReadOnlyList<HonkModInfo> mods = HonkModLoader.ModInfos;
            if (mods.Count > MaxVisibleMods && scrollOffset > 0)
            {
                scrollOffset--;
                RefreshModListText();
            }
        }

        public void ScrollDown()
        {
            IReadOnlyList<HonkModInfo> mods = HonkModLoader.ModInfos;
            int maxOffset = Math.Max(0, mods.Count - MaxVisibleMods);
            if (scrollOffset < maxOffset)
            {
                scrollOffset++;
                RefreshModListText();
            }
        }

        public void RefreshModListText()
        {
            IReadOnlyList<HonkModInfo> mods = HonkModLoader.ModInfos;
            int total = mods.Count;

            LocalizationLanguage lang = LocalizationLanguage.brazilianPortuguese;
            try
            {
                lang = LocalizationManager.GetLanguage();
            }
            catch { }

            int maxOffset = Math.Max(0, total - MaxVisibleMods);
            if (scrollOffset > maxOffset) scrollOffset = maxOffset;
            if (scrollOffset < 0) scrollOffset = 0;

            StringBuilder sb = new StringBuilder();

            if (total == 0)
            {
                sb.AppendLine(GetNoModsText(lang));
                sb.AppendLine(string.Format("<size=70%>{0}</size>", GetFolderText(lang)));
            }
            else
            {
                int end = Math.Min(scrollOffset + MaxVisibleMods, total);
                for (int i = scrollOffset; i < end; i++)
                {
                    HonkModInfo m = mods[i];
                    sb.AppendLine(string.Format("{0}. {1} <size=75%>v{2}</size>", i + 1, m.Name, m.Version));
                }

                if (total > MaxVisibleMods)
                {
                    sb.AppendLine(string.Format("<size=70%>{0}</size>", GetPaginationText(lang, scrollOffset + 1, end, total)));
                }
                else
                {
                    sb.AppendLine(string.Format("<size=70%>{0}</size>", GetFolderText(lang)));
                }
            }

            if (contentTMP != null)
            {
                string text = sb.ToString();
                try
                {
                    text = LocalizationManager.GetTransferedText(text, contentTMP);
                }
                catch { }
                contentTMP.text = text;
                contentTMP.ForceMeshUpdate();
            }

            if (backBtnTMP != null)
            {
                string backText = GetBackText(lang);
                try
                {
                    backText = LocalizationManager.GetTransferedText(backText, backBtnTMP);
                }
                catch { }
                backBtnTMP.text = backText;
                backBtnTMP.ForceMeshUpdate();
            }

            UpdateBackBtnPosition(total);
        }

        private void UpdateBackBtnPosition(int total)
        {
            if (backBtnObj == null) return;

            float targetY;
            if (total <= 0)
            {
                targetY = -130f;
            }
            else if (total == 1)
            {
                targetY = -165f;
            }
            else if (total == 2)
            {
                targetY = -200f;
            }
            else
            {
                targetY = -235f;
            }

            // Trava para NUNCA sair da tela da placa
            if (targetY < -240f)
            {
                targetY = -240f;
            }

            backBtnObj.transform.localPosition = new Vector3(0f, targetY, 0f);
        }

        public void GoToMainMenu()
        {
            base.enabled = false;
            gameObject.SetActive(false);
            UIInputManager.Unsubscribe(this);

            if (MainMenu.instance != null)
            {
                if (MainMenu.instance.mainMenuSwappable != null)
                {
                    MainMenu.instance.mainMenuSwappable.gameObject.SetActive(true);
                }
                MainMenu.instance.enabled = true;
                if (MainMenuBase.instance != null)
                {
                    MainMenuBase.instance.currentScreen = MainMenu.instance;
                }
                UIInputManager.Subscribe(MainMenu.instance);
                if (MainMenu.instance.littleMenu != null)
                {
                    MainMenu.instance.littleMenu.SetCurrentSelection(3, silently: false);
                }
            }
        }

        public void Up()
        {
            ScrollUp();
        }

        public void Down()
        {
            ScrollDown();
        }

        public void Left()
        {
        }

        public void Right()
        {
        }

        public void Accept()
        {
            if (littleMenu != null)
            {
                littleMenu.Select();
            }
            else
            {
                GoToMainMenu();
            }
        }

        public void Back()
        {
            GoToMainMenu();
        }

        public void Esc()
        {
            GoToMainMenu();
        }

        public void Tertiary()
        {
        }

        public void Pause()
        {
        }

        public void Goals()
        {
        }

        private void OnDisable()
        {
            UIInputManager.Unsubscribe(this);
        }
    }

    public static class HonkMainMenuUI
    {
        private static MainMenu currentMainMenuInstance;
        private static GameObject modsButtonObj;
        private static GameObject modsMenuObj;
        private static HonkModsMenuScreen modsMenuScreen;

        private static void DisableLocalization(GameObject obj)
        {
            if (obj == null) return;
            LocalizedText[] locs = obj.GetComponentsInChildren<LocalizedText>(true);
            for (int i = 0; i < locs.Length; i++)
            {
                locs[i].StringTableId = "";
                locs[i].dontLocalize = true;
                locs[i].enabled = false;
                UnityEngine.Object.Destroy(locs[i]);
            }
        }

        public static void SetupModsMenu(MainMenu mainMenu)
        {
            if (mainMenu == null) return;
            if (currentMainMenuInstance == mainMenu && modsButtonObj != null) return;

            try
            {
                currentMainMenuInstance = mainMenu;

                if (mainMenu.creditsHighlightElement == null || mainMenu.quitHighlightElement == null) return;

                Transform creditsDotTrans = mainMenu.creditsHighlightElement.transform;
                Transform quitDotTrans = mainMenu.quitHighlightElement.transform;

                Transform creditsObj = creditsDotTrans.parent;
                Transform quitObj = quitDotTrans.parent;

                if (creditsObj == null || quitObj == null) return;

                // 1. Desloca o botao Quit para a posicao 4 (-152 em relacao a Credits)
                quitObj.localPosition = new Vector3(0f, -152f, 0f);
                quitObj.localRotation = Quaternion.identity;
                quitObj.localScale = Vector3.one;

                // 2. Clona Quit para criar Mods na posicao 3 (-76 em relacao a Credits)
                modsButtonObj = UnityEngine.Object.Instantiate(quitObj.gameObject, creditsObj);
                modsButtonObj.name = "Mods";
                modsButtonObj.transform.localPosition = new Vector3(0f, -76f, 0f);
                modsButtonObj.transform.localRotation = Quaternion.identity;
                modsButtonObj.transform.localScale = Vector3.one;
                modsButtonObj.SetActive(true);

                // Desativa localizacao em Mods
                DisableLocalization(modsButtonObj);

                TMP_Text modsTmp = modsButtonObj.GetComponent<TMP_Text>();
                if (modsTmp != null)
                {
                    modsTmp.text = "Mods";
                    modsTmp.ForceMeshUpdate();
                }

                GameObject modsDot = null;
                Transform dotTrans = modsButtonObj.transform.Find(quitDotTrans.name);
                if (dotTrans != null)
                {
                    modsDot = dotTrans.gameObject;
                    modsDot.name = "ModsDot";
                    modsDot.transform.localPosition = new Vector3(-35f, 0f, 0f);
                    modsDot.SetActive(false);
                }

                // Ajusta indices dos colisores de mouse
                MouseableMenuCollider quitCol = quitObj.GetComponentInChildren<MouseableMenuCollider>(true);
                if (quitCol != null)
                {
                    quitCol.selection = 4;
                }

                MouseableMenuCollider modsCol = modsButtonObj.GetComponentInChildren<MouseableMenuCollider>(true);
                if (modsCol != null)
                {
                    modsCol.name = "mouseableMods";
                    modsCol.selection = 3;
                    modsCol.menu = mainMenu;
                    modsCol.menuMouseable = mainMenu;
                }

                // Insere comando e highlight no LittleMenu
                if (mainMenu.littleMenu != null && mainMenu.littleMenu.menuCommands != null)
                {
                    if (!mainMenu.littleMenu.menuCommands.Contains(OpenModsMenu))
                    {
                        mainMenu.littleMenu.menuCommands.Insert(3, OpenModsMenu);
                        if (mainMenu.littleMenu.highlightElements != null)
                        {
                            mainMenu.littleMenu.highlightElements.Insert(3, modsDot != null ? modsDot : modsButtonObj);
                        }
                        if (mainMenu.littleMenu.optionsEnabledState != null && mainMenu.littleMenu.optionsEnabledState.Count >= 3)
                        {
                            mainMenu.littleMenu.optionsEnabledState.Insert(3, true);
                        }
                    }
                }

                CreateModsMenuScreen(mainMenu, modsButtonObj);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("HonkAPI: Failed to setup Mods menu in MainMenu: " + ex);
            }
        }

        private static void CreateModsMenuScreen(MainMenu mainMenu, GameObject buttonTemplate)
        {
            if (mainMenu == null || buttonTemplate == null) return;

            Transform titleScreenTrans = mainMenu.transform;

            modsMenuObj = new GameObject("ModsMenuSwappable");
            modsMenuObj.transform.SetParent(titleScreenTrans, false);
            modsMenuObj.transform.localPosition = new Vector3(-43f, -95f, -30f);
            modsMenuObj.transform.localRotation = Quaternion.identity;
            modsMenuObj.transform.localScale = Vector3.one;

            modsMenuScreen = modsMenuObj.AddComponent<HonkModsMenuScreen>();

            // 1. Titulo: "Mods"
            GameObject titleObj = UnityEngine.Object.Instantiate(buttonTemplate, modsMenuObj.transform);
            titleObj.name = "ModsTitle";
            titleObj.transform.localPosition = new Vector3(0f, 35f, 0f);
            titleObj.transform.localRotation = Quaternion.identity;
            titleObj.transform.localScale = Vector3.one;
            DisableLocalization(titleObj);

            MouseableMenuCollider tCol = titleObj.GetComponentInChildren<MouseableMenuCollider>(true);
            if (tCol != null) UnityEngine.Object.Destroy(tCol.gameObject);
            Transform tDot = titleObj.transform.Find("ModsDot");
            if (tDot != null) UnityEngine.Object.Destroy(tDot.gameObject);

            TMP_Text titleTmp = titleObj.GetComponent<TMP_Text>();
            if (titleTmp != null)
            {
                titleTmp.text = "Mods";
                titleTmp.ForceMeshUpdate();
            }

            // 2. Conteudo: lista de mods ativos (pivot no topo)
            GameObject contentObj = UnityEngine.Object.Instantiate(titleObj, modsMenuObj.transform);
            contentObj.name = "ModsContent";
            contentObj.transform.localPosition = new Vector3(0f, -25f, 0f);
            contentObj.transform.localRotation = Quaternion.identity;
            contentObj.transform.localScale = Vector3.one;

            modsMenuScreen.contentTMP = contentObj.GetComponent<TMP_Text>();
            if (modsMenuScreen.contentTMP != null)
            {
                if (titleTmp != null)
                {
                    modsMenuScreen.contentTMP.fontSize = titleTmp.fontSize * 0.55f;
                }
                if (modsMenuScreen.contentTMP.rectTransform != null)
                {
                    modsMenuScreen.contentTMP.rectTransform.pivot = new Vector2(0.5f, 1f);
                    modsMenuScreen.contentTMP.rectTransform.sizeDelta = new Vector2(250f, 160f);
                }
                modsMenuScreen.contentTMP.alignment = TextAlignmentOptions.TopLeft;
            }

            // 3. Botao: "Voltar" (posicao inicial, sera atualizada dinamicamente em UpdateBackBtnPosition)
            GameObject backBtn = UnityEngine.Object.Instantiate(buttonTemplate, modsMenuObj.transform);
            backBtn.name = "BackBtn";
            backBtn.transform.localPosition = new Vector3(0f, -140f, 0f);
            backBtn.transform.localRotation = Quaternion.identity;
            backBtn.transform.localScale = Vector3.one;
            DisableLocalization(backBtn);

            TMP_Text backTmp = backBtn.GetComponent<TMP_Text>();
            if (backTmp != null)
            {
                backTmp.text = "Voltar";
                backTmp.ForceMeshUpdate();
            }

            MouseableMenuCollider bCol = backBtn.GetComponentInChildren<MouseableMenuCollider>(true);
            if (bCol != null)
            {
                bCol.name = "mouseableBack";
                bCol.selection = 0;
                bCol.menu = modsMenuScreen;
                bCol.menuMouseable = modsMenuScreen;
            }

            Transform bDot = backBtn.transform.Find("ModsDot");
            if (bDot != null)
            {
                bDot.name = "BackDot";
                bDot.localPosition = new Vector3(-35f, 0f, 0f);
                bDot.gameObject.SetActive(false);
                modsMenuScreen.backHighlight = bDot.gameObject;
            }
            else
            {
                modsMenuScreen.backHighlight = backBtn;
            }

            modsMenuScreen.backBtnObj = backBtn;
            modsMenuScreen.backBtnTMP = backTmp;
            modsMenuScreen.Initialize();
            modsMenuObj.SetActive(false);
        }

        public static void OpenModsMenu()
        {
            if (MainMenu.instance == null || modsMenuScreen == null) return;

            if (MainMenu.instance.mainMenuSwappable != null)
            {
                MainMenu.instance.mainMenuSwappable.gameObject.SetActive(false);
            }

            MainMenu.instance.enabled = false;
            UIInputManager.Unsubscribe(MainMenu.instance);

            modsMenuScreen.Activate();
        }

        public static void EnsureModsMenu(MainMenu mainMenu)
        {
            if (currentMainMenuInstance != mainMenu || modsButtonObj == null)
            {
                SetupModsMenu(mainMenu);
            }
        }
    }
}
