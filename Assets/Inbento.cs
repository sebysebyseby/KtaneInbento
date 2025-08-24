using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class Inbento : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public Texture[] GameTiles;
   public Texture[] SolutionTiles;

   public GameObject[] SolutionTileObjects;

   public Material[] tileMaterials;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   private  bool test;

   int[][] Solution;

   void Awake () { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      Debug.Log("Awake called");
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      //button.OnInteract += delegate () { buttonPress(); return false; };

   }

   void OnDestroy () { //Shit you need to do when the bomb ends
      
   }

   void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on

   }

   void Start () { //Shit that you calculate, usually a majority if not all of the module

      // Just set a simple texture
      // foreach (GameObject tileObject in SolutionTileObjects) {
      //    tileObject.GetComponent<MeshRenderer>().material = tileMaterials[0];
      // }

      // Create a random desired solution
      Solution = new int[3][];
      for (int i = 0; i < 3; i++) {
         Solution[i] = new int[3];
         for (int j = 0; j < 3; j++) {
            int randomIndex = Rnd.Range(0, tileMaterials.Length);
            Debug.Log("i: " + i + " j: " + j + " randomIndex: " + randomIndex);

            Solution[i][j] = randomIndex;
            var tileObject = SolutionTileObjects[i * 3 + j];
            tileObject.GetComponent<MeshRenderer>().material = tileMaterials[randomIndex];
         }
      }
      Debug.Log("Solution: " + Solution[0][0] + " " + Solution[0][1] + " " + Solution[0][2] + " " + Solution[1][0] + " " + Solution[1][1] + " " + Solution[1][2] + " " + Solution[2][0] + " " + Solution[2][1] + " " + Solution[2][2]);
   }

   public class Shape { //random comment here.
      public string id;
      public string size;

      public Shape(string id, string size) {
         this.id = id;
         this.size = size;
      }
   }

   // Shape definitions:
   // s (stands for shape)
   // # (# of tiles in the shape)
   // x (arbitrary separator)
   // #### (tiles in the shape, as though they were in a 3x3 grid starting on 1 and going reading order)

   // 1-tile shapes
   static Shape s1x1 = new Shape("test", "1x1");
   // 2-tile shapes
   static Shape s2x12 = new Shape("test", "2x1");
   static Shape s2x13 = new Shape("test", "3x1");
   static Shape s2x15 = new Shape("test", "2x2");
   static Shape s2x16 = new Shape("test", "3x2");
   static Shape s2x18 = new Shape("test", "2x3");
   static Shape s2x19 = new Shape("test", "3x3");
   // 3-tile shapes
   static Shape s3x123 = new Shape("test", "3x1");
   static Shape s3x124 = new Shape("test", "2x2");
   static Shape s3x126 = new Shape("test", "3x2");
   static Shape s3x127 = new Shape("test", "2x3");
   static Shape s3x128 = new Shape("test", "2x3");
   static Shape s3x129 = new Shape("test", "3x3");
   static Shape s3x135 = new Shape("test", "3x2");
   static Shape s3x137 = new Shape("test", "3x3");
   static Shape s3x138 = new Shape("test", "3x3");
   static Shape s3x148 = new Shape("test", "2x3");
   static Shape s3x149 = new Shape("test", "3x3");
   static Shape s3x159 = new Shape("test", "3x3");
   // 4-tile shapes
   static Shape s4x1234 = new Shape("test", "3x2");
   static Shape s4x1235 = new Shape("test", "3x2");
   static Shape s4x1236 = new Shape("test", "3x2");
   static Shape s4x1237 = new Shape("test", "3x3");
   static Shape s4x1238 = new Shape("test", "3x3");
   static Shape s4x1239 = new Shape("test", "3x3");
   static Shape s4x1245 = new Shape("test", "2x2");
   static Shape s4x1246 = new Shape("test", "3x2");
   static Shape s4x1248 = new Shape("test", "2x3");
   static Shape s4x1249 = new Shape("test", "3x3");
   static Shape s4x1256 = new Shape("test", "3x2");
   static Shape s4x1259 = new Shape("test", "3x3");
   static Shape s4x1267 = new Shape("test", "3x3");
   static Shape s4x1268 = new Shape("test", "3x3");
   static Shape s4x1269 = new Shape("test", "3x3");
   static Shape s4x1278 = new Shape("test", "2x3");
   static Shape s4x1279 = new Shape("test", "3x3");
   static Shape s4x1289 = new Shape("test", "3x3");
   static Shape s4x1348 = new Shape("test", "3x3");
   static Shape s4x1349 = new Shape("test", "3x3");
   static Shape s4x1357 = new Shape("test", "3x3");
   static Shape s4x1358 = new Shape("test", "3x3");
   static Shape s4x1379 = new Shape("test", "3x3");
   static Shape s4x1458 = new Shape("test", "2x3");
   static Shape s4x1459 = new Shape("test", "3x3");
   static Shape s4x1468 = new Shape("test", "3x3");
   static Shape s4x1469 = new Shape("test", "3x3");
   static Shape s4x1568 = new Shape("test", "3x3");
   static Shape s4x2468 = new Shape("test", "3x3");

   // TODO - I should be able to dynamically create shapes, but not needed for now





   void Update () { //Shit that happens at any point after initialization

   }

   void Solve () {
      GetComponent<KMBombModule>().HandlePass();
   }

   void Strike () {
      GetComponent<KMBombModule>().HandleStrike();
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
