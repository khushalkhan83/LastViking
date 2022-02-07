using System.Collections.Generic;
using DebugActions;
using UnityEngine;

namespace ActionsCollections.UI
{
    public class DebugActionsController : MonoBehaviour
    {
        [SerializeField] private SectionsPanel sectionsPanel;
        [SerializeField] private ActionsPanel actionsPanel;

        private IActionCollection debugSectionsProvider = DebugActionCollection.Instance;
        private List<SectionBase> Sections => debugSectionsProvider.Sections;

        private void Start()
        {
            DisplaySections();
        }

        private void DisplaySections()
        {
            sectionsPanel.gameObject.SetActive(true);
            actionsPanel.gameObject.SetActive(false);

            sectionsPanel.SetData(Sections, OnSectionSelected);
        }

        private void DisplaySection(SectionBase selectedSection)
        {
            sectionsPanel.gameObject.SetActive(false);
            actionsPanel.gameObject.SetActive(true);

            actionsPanel.SetData(selectedSection, OnBackButton);
        }



        private void OnSectionSelected(SectionBase selectedSection) => DisplaySection(selectedSection);
        private void OnBackButton() => DisplaySections();
    }
}
