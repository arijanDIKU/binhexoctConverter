open System.Windows.Forms
open System.Drawing
open System 

////////////////////////////
///// FORMS AND PANELS /////
////////////////////////////
let win = new System.Windows.Forms.Form () 
let mainPanel = new FlowLayoutPanel()


let buttonPanel = new FlowLayoutPanel()
let numberSysPanel = new FlowLayoutPanel()
let operatorPanel = new FlowLayoutPanel()
let inputLine = new TextBox()





////////////////////////////////////////////
///// EVENT HANDLING AND FUNCTIONALITY /////   
////////////////////////////////////////////




///////////////////////////////////////////////////////
///// BUTTONS, INITIALIZATIONS AND EVENT HANDLING /////
///////////////////////////////////////////////////////

let numbersNoperators = 
  [(new Button(), "7"); (new Button(), "8"); (new Button(), "9");
   (new Button(), "4"); (new Button(), "5"); (new Button(), "6");
   (new Button(), "1"); (new Button(), "2"); (new Button(), "3");
                        (new Button(), "0");
   (new Button(), "+"); (new Button(), "-"); (new Button(), "*"); (new Button(), "/")] 
numbersNoperators |> List.iter (fun (btn, txt) -> btn.Text <- txt; btn.Size <- new Size (30,20); buttonPanel.Controls.Add btn; 
                                                  btn.Click.Add (fun _ -> inputLine.AppendText(txt)) )

let numSystems = [(new RadioButton(), "bin"); (new RadioButton(), "oct"); 
                  (new RadioButton(), "dec"); (new RadioButton(), "hex")] 
let mutable currentSystem = "bin" //default 
let (b,_) = numSystems |> List.head in b.Checked <- true //default 
numSystems |> List.iter (fun (btn, txt) -> btn.Text <- txt; numberSysPanel.Controls.Add btn; 
                                           btn.CheckedChanged.Add (fun _ -> currentSystem <- txt))


//////////////////////////
///// BACK END ///////////
//////////////////////////

let parseNCompute input action = 
   let validOperators = ['-';'+';'*';'/']
   let checkNumberSystem input = match currentSystem with 
                                  | "bin" -> Seq.forall (fun c -> c='1' || c='0') input
                                  | "oct" -> Seq.forall (fun c -> List.contains c [for i in 0..8 -> char i]) input
                                  | "dec" -> Seq.forall (fun c -> List.contains c [for i in 0..9 -> char i]) input
                                  | "hex" -> Seq.forall (fun c -> List.contains c [for i in 0..9 -> char i]@['a'..'f']@['A'..'F']) input

   let check input = // check that only numbers and symbols are included and that numbersystems matches checked box
      if Seq.forall (fun c -> List.contains c validOperators) && 
         Seq.pairwise input |> Seq.forall (fun (c1,c2) -> not System.Char.IsDigit c1 && not System.Char.IsDigit c2) &&
         check checkNumberSystem && not (List.contains input.[0] validOperators) then 
           true //valid
      else 
           false //invalid


    let parse input = 
       let splitArgs = Array.ofList validOperators 
       let digitDelims () = match currentSystem with | "bin" -> [|'1';'0'|] | "oct" -> [|'0'..'8'|] | "dec" -> [|'0'..'9'|] 
                                                     | "hex" -> Array.append [|'0'..'9'|] [|'a'..'f'|] |> Array.append [|'A'..'F'|]
       let operands, operators = input.Split('*', '/') |> Array.map (fun s -> input.Split('+', '-'))
                                 input.Split(digitDelims (), System.StringSplitOptions.RemoveEmptyEntries)
       Array.map (fun S -> Array.map (fun s -> int s) S) operands, Array.map (function | '*' -> (*) | '/' -> (/) | '+' -> (+) | '-' -> (-)) operators
         

      

let operators = 
  [(new Button (), "=");
   (new Button (), "To bin"); (new Button (), "To oct"); (new Button (), "To dec"); (new Button (), "To hex")] 
operators |> List.iter (fun (btn, txt) -> btn.Text <- txt; btn.Size <- new Size (90,20); operatorPanel.Controls.Add btn;
                                          btn.Click.Add (fun _ -> 
                                                                  let input = inputLine.Text in 
                                                                  let result = parseNCompute action () in inputLine.Text <- result))



   


////////////////////////////////
///// VIEWS AND APPEARANCE /////
////////////////////////////////

buttonPanel.Location <- new Point (5,110)
buttonPanel.BorderStyle <- BorderStyle.Fixed3D
buttonPanel.Size <- new Size (120,245)
buttonPanel.WrapContents <- true 

numberSysPanel.Location <- new Point (5,45)
numberSysPanel.BorderStyle <- BorderStyle.Fixed3D
numberSysPanel.Size <- new Size (225, 60)
numberSysPanel.WrapContents <- true 

operatorPanel.Location <- new Point (130,110)
operatorPanel.BorderStyle <- BorderStyle.Fixed3D
operatorPanel.Size <- new Size (100,245)
operatorPanel.WrapContents <- true


inputLine.Location <- new Point (5,5)
inputLine.Size <- new Size(225,0)


mainPanel.Controls.Add inputLine
mainPanel.Controls.Add numberSysPanel
mainPanel.Controls.Add buttonPanel
mainPanel.Controls.Add operatorPanel



mainPanel.Location <- new Point (5,25)
mainPanel.BorderStyle <- BorderStyle.Fixed3D
mainPanel.ClientSize <- new Size (235,350)
mainPanel.WrapContents <- true


///////////////////////////
///// RUN APPLICATION /////
///////////////////////////

win.Controls.Add mainPanel
win.ClientSize <- new Size (500,500)
win.Text <- "Converting calculator"
Application.Run win