using UnityEngine;

[CreateAssetMenu(fileName = "PrefabNamingConvention", menuName = "Documentation/Prefab Naming Convention")]
public class PrefabNamingConvention : ScriptableObject
{
  [Header("Prefab Naming Convention")]
  [TextArea(25, 35)]
  public string conventionText =
      "<b><size=16>Unity Prefab Naming Convention</size></b>\n\n" +
      "This naming convention serves as a practical middle ground between Unity and Unreal Engine conventions. " +
      "The prefixes act as quick visual hints to help team members and developers instantly understand what type of " +
      "prefab they're working with, without needing to open it first.\n\n" +
      "<b><size=14>Prefix Guide</size></b>\n\n" +
      "<b>P_</b> - Standard Prefab\n" +
      "A basic prefab containing any general game object or component setup.\n\n" +
      "<b>P_V_</b> - Prefab Variant\n" +
      "A variant of an existing prefab with modified properties or components.\n\n" +
      "<b>PA_</b> - Prefab Actor\n" +
      "A prefab representing an interactive game entity or character (borrowed from Unreal's \"Actor\" concept).\n\n" +
      "<b>PAV_</b> - Prefab Actor Variant\n" +
      "A variant of an Actor prefab with customized settings.\n\n" +
      "<b>PP_</b> - Prefab Pawn\n" +
      "A prefab for controllable characters or entities (using Unreal's \"Pawn\" terminology for player-controllable objects).\n\n" +
      "<b>PAI_</b> - Prefab AI-Controlled Pawn\n" +
      "A prefab specifically for AI-controlled characters, making it clear this entity uses autonomous behavior.\n\n" +
      "<b>PUI_</b> - Prefab UI\n" +
      "A prefab containing user interface elements.\n\n" +
      "<i>Note: These prefixes are organizational tools to improve workflow efficiency. They give you an at-a-glance " +
      "understanding of a prefab's purpose and contents, saving time during development and collaboration.</i>";
}
