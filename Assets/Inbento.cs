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
   public GameObject[] BoardTileObjects;

   public GameObject helloWorldText;

   // These objects will hold all the tiles, so you can easily add add and position tiles, then rotate the whole shape
   public GameObject pieceContainer1;
   public GameObject pieceContainer2;
   public GameObject pieceContainer3;
   public GameObject pieceContainer4;


   public GameObject puzzlePiece1x1Template;

   public Material[] tileMaterials; // 0 is reserved as an empty value

   public KMSelectable undoButton;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   int[][] Solution;
   int[][] BoardState;
   static int boardWidth = 3; // in case I want to increase the size of the grid later
   static int boardHeight = 3;
   static int numberOfPieces = 4;

   private List<GameObject> pieceContainers = new List<GameObject>();
   private List<Piece> pieces = new List<Piece>();

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

      
      undoButton.OnInteract += delegate () { pressUndoButton(); return false; };

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

      // initialize the piece container list
      pieceContainers.AddRange(new GameObject[] { pieceContainer1, pieceContainer2, pieceContainer3, pieceContainer4 });
      


      // Create a random solution
      Solution = createEmptyTiles();
      for (int i = 0; i < boardHeight; i++) {
         for (int j = 0; j < boardWidth; j++) {
            var materialIndex = Rnd.Range(1, tileMaterials.Length);
            Solution[i][j] = materialIndex;
            var tileObject = SolutionTileObjects[i * 3 + j];
            tileObject.GetComponent<MeshRenderer>().material = tileMaterials[materialIndex];
         }
      }

      // clone the solution so I don't accidentally modify the original
      BoardState = createEmptyTiles();
      for (int i = 0; i < boardHeight; i++) {
         for (int j = 0; j < boardWidth; j++) {
            BoardState[i][j] = Solution[i][j];
         }
      }

      // Next, create a new piece.
      for (int i = 0; i < numberOfPieces; i++) {
         // creates the piece
         Piece generatedPiece = generatePiece(BoardState);
         // updates the board state
         for (int j = 0; j < boardHeight; j++) {
            for (int k = 0; k < boardWidth; k++) {
               if (generatedPiece.tiles[j][k] != 0) {
                  var material = generatedPiece.tiles[j][k];
                  var newMaterial = material;
                  while (newMaterial == material) {
                     newMaterial = Rnd.Range(1, tileMaterials.Length);
                  }
                  BoardState[j][k] = newMaterial;
               }
            }
         }
         pieces.Add(generatedPiece);
      }

      // Draw the board state
      for (int i = 0; i < boardHeight; i++) {
         for (int j = 0; j < boardWidth; j++) {
            var materialIndex = BoardState[i][j];
            var tileObject = BoardTileObjects[i * 3 + j];
            tileObject.GetComponent<MeshRenderer>().material = tileMaterials[materialIndex];
         }
      }

      // Draw the pieces
      for (int i = 0; i < numberOfPieces; i++) {
         var pieceContainer = pieceContainers[i];
         var piece = pieces[i];

         var onexoneMaterial = 0;
         for (int j = 0; j < boardHeight; j++) {
            for (int k = 0; k < boardWidth; k++) {
               if (piece.tiles[j][k] != 0) {
                  onexoneMaterial = piece.tiles[j][k];
               }
            }
         }

         Debug.Log(pieceContainer);
         
         var newPiece = Instantiate(puzzlePiece1x1Template, pieceContainer.transform.position, Quaternion.identity);
         newPiece.transform.parent = pieceContainer.transform;
         newPiece.transform.localPosition = new Vector3(puzzlePiece1x1Template.transform.localPosition.x, puzzlePiece1x1Template.transform.localPosition.y, puzzlePiece1x1Template.transform.localPosition.z);
         newPiece.transform.localRotation = Quaternion.identity;
         newPiece.transform.localScale = new Vector3(puzzlePiece1x1Template.transform.localScale.x, puzzlePiece1x1Template.transform.localScale.y, puzzlePiece1x1Template.transform.localScale.z);
         newPiece.GetComponent<MeshRenderer>().material = tileMaterials[onexoneMaterial];
      }

      testFunction();
   }

   // A function I use to test misc stuff
   void testFunction()
   {
      // I need to instantiate a new 1x1 piece, and make it the child of the piece1 game object
      // this should work exactly as though I dragged the 1x1 game object into the piece 1 game object
      // GameObject newPiece = Instantiate(puzzlePiece1x1Template, piece1.transform.position, Quaternion.identity);
      // newPiece.transform.parent = piece1.transform;
      // newPiece.transform.localPosition = new Vector3(puzzlePiece1x1Template.transform.localPosition.x, puzzlePiece1x1Template.transform.localPosition.y, puzzlePiece1x1Template.transform.localPosition.z);
      // newPiece.transform.localRotation = Quaternion.identity;
      // newPiece.transform.localScale = new Vector3(puzzlePiece1x1Template.transform.localScale.x, puzzlePiece1x1Template.transform.localScale.y, puzzlePiece1x1Template.transform.localScale.z);
      // newPiece.GetComponent<MeshRenderer>().material = tileMaterials[0];


      // Instantiate a 2x1 piece -- successful so far
      // GameObject newPiece2 = Instantiate(puzzlePiece1x1Template, piece1.transform.position, Quaternion.identity);
      // newPiece2.transform.parent = piece1.transform;
      // newPiece2.transform.localPosition = new Vector3(puzzlePiece1x1Template.transform.localPosition.x + 0.15f, puzzlePiece1x1Template.transform.localPosition.y, puzzlePiece1x1Template.transform.localPosition.z);
      // newPiece2.transform.localScale = new Vector3(puzzlePiece1x1Template.transform.localScale.x, puzzlePiece1x1Template.transform.localScale.y, puzzlePiece1x1Template.transform.localScale.z);
      // newPiece2.GetComponent<MeshRenderer>().material = tileMaterials[0];

      // GameObject newPiece3 = Instantiate(puzzlePiece1x1Template, piece1.transform.position, Quaternion.identity);
      // newPiece3.transform.parent = piece1.transform;
      // newPiece3.transform.localPosition = new Vector3(puzzlePiece1x1Template.transform.localPosition.x - 0.15f, puzzlePiece1x1Template.transform.localPosition.y, puzzlePiece1x1Template.transform.localPosition.z);
      // newPiece3.transform.localScale = new Vector3(puzzlePiece1x1Template.transform.localScale.x, puzzlePiece1x1Template.transform.localScale.y, puzzlePiece1x1Template.transform.localScale.z);
      // newPiece3.GetComponent<MeshRenderer>().material = tileMaterials[0];

      // this I cannot do.
      // newPiece3.transform.localPosition.x += 0.4f;
   }

   // To generate a piece:
   // - pick a random shape
   // - "fit it" randomly on the solution grid to determine what the tiles should be
   // - return the piece
   public Piece generatePiece(int[][] solution) {
      // for now, always do 1x1 piece
      // choose a random tile in the solution grid
      int row = Rnd.Range(0, 3);
      int column = Rnd.Range(0, 3);
      int tile = solution[row][column];

      // create a new set of tiles using the randomly picked solution tile
      var tiles = createEmptyTiles();
      tiles[column][row] = tile;

      var piece = new Piece(tiles, "1x1");
      Debug.Log("generated piece: " + piece.tiles[0][0] + " " + piece.tiles[0][1] + " " + piece.tiles[0][2] + " " + piece.tiles[1][0] + " " + piece.tiles[1][1] + " " + piece.tiles[1][2] + " " + piece.tiles[2][0] + " " + piece.tiles[2][1] + " " + piece.tiles[2][2]);
      return piece;

      // TODO: make it return more than 1x1
      // For now, I'll always pick 1x1 instead of
      // create copy of s1x1
      var newShape = new Shape(createEmptyTiles(), "1x1");

      if (newShape.size == "1x1") {
      }

      if (newShape.size == "2x1" || newShape.size == "1x2") {

      }
      return null;
   }

   public int[][] createEmptyTiles() {
      var tiles = new int[boardHeight][];
      for (int i = 0; i < boardHeight; i++) {
         tiles[i] = new int[boardWidth];
      }
      return tiles;
   }


   public class Shape
   { //random comment here.
      public int[][] tiles;
      public string size;

      public Shape(int[][] tiles, string size)
      {
         this.tiles = tiles;
         this.size = size;
      }

      // public int 
   }

   public class Piece
   {
      public int[][] tiles;
      public string size;
      public int rotation;
      public string position;

      public Piece(int[][] tiles, string size, int rotation = 0, string position = null)
      {
         this.tiles = tiles;
         this.size = size;
         this.rotation = rotation;
         this.position = position;
      }

   }

   // Shape definitions:
   // s (stands for shape)
   // # (# of tiles in the shape)
   // x (arbitrary separator)
   // #### (tiles in the shape, as though they were in a 3x3 grid starting on 1 and going reading order)


   /*
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
   */

   // TODO - I should be able to dynamically create shapes, but not needed for now



   void pressUndoButton () {
      Debug.Log("pressUndoButton called");
      GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      GetComponent<KMSelectable>().AddInteractionPunch(0.75f);
      // // move the hellow world text 0.1 units to the left
      // helloWorldText.GetComponent<Transform>().position = new Vector3(helloWorldText.GetComponent<Transform>().position.x - 0.008f, helloWorldText.GetComponent<Transform>().position.y, helloWorldText.GetComponent<Transform>().position.z);
   }

   // Rotates the piece 90 degrees clockwise
   // Rotates the object containing all the individual tiles for smooth and simple rotation
   void rotatePiece(GameObject piece) {
      piece.transform.GetChild(0).transform.Rotate(0, 90, 0);
   }

   void hidePiece(GameObject piece) {
      // hide the meshrenderer of all children with a foreach loop
      foreach (Transform child in piece.transform) {
         child.GetComponent<MeshRenderer>().enabled = false;
      }
   }

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
