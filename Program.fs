open System.Windows.Forms
open System.Drawing
open System 

let win = new System.Windows.Forms.Form () 	 //main
let numberPanel = new FlowLayoutPanel()          //input
let numericalSystemPanel = new FlowLayoutPanel() //bin,hex,oct,dec 
let operatorPanel = new FlowLayoutPanel() 	 //+ - * /



let numbers = 
  [for n in 0..9 do yield (new Button (), string n)]
let numSystems =
  [(new RadioButton (), "bin"); (new Radioutton (), "oct"); 
   (new RadioButton (), "dec"); (new RadioButton(), "hex")]
let operators = 
  [(new Button (), "+"); (new Button (), "-"); (new Button (), "*"); (new Button(), "/")]

numbers::numSystems::operators::[] |>
 List.iter (fun btnList -> for (btn,txt) in btnList do yield btn.Text <- txt)



//Application.Run win