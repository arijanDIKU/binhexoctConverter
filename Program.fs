open System.Windows.Forms
open System.Drawing
open System 
open FParsec 
open Expressions



////////////////////////////
///// FORMS AND PANELS /////
////////////////////////////
let win = new System.Windows.Forms.Form () 
let mainPanel = new FlowLayoutPanel()


let buttonPanel = new FlowLayoutPanel()
let numberSysPanel = new FlowLayoutPanel()
let operatorPanel = new FlowLayoutPanel()
let inputLine = new TextBox()




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
                                           btn.CheckedChanged.Add (fun _ -> currentSystem <- txt)) |> ignore



//////////////////////////
///// BACK END ///////////
//////////////////////////


let isSymbolicOperatorChar = isAnyOf "*+-/"
let remainingOpChars_ws = manySatisfy isSymbolicOperatorChar .>> spaces
 
let opp = new OperatorPrecedenceParser<Expr, string,unit>()


opp.TermParser <- pint32 .>> spaces |>> Num


opp.AddOperator(InfixOperator("+",  remainingOpChars_ws, 10, Associativity.Left, (), fun remOpChars e1 e2 -> Add(e1,e2)) )
opp.AddOperator(InfixOperator("-",  remainingOpChars_ws, 10, Associativity.Left, (), fun remOpChars e1 e2 -> Sub(e1,e2)) )
opp.AddOperator(InfixOperator("*",  remainingOpChars_ws, 20, Associativity.Left, (), fun remOpChars e1 e2 -> Mult(e1,e2)) )
opp.AddOperator(InfixOperator("/",  remainingOpChars_ws, 20, Associativity.Left, (), fun remOpChars e1 e2 -> Div(e1,e2)) )



let eval presult = 
   let rec eval' exp =   
       match exp with 
       | Num n -> n 
       | Add(n,m) -> eval' n + eval' m
       | Sub(n,m) -> eval' n - eval' m
       | Mult(n,m) -> eval' n * eval' m
       | Div(n,m) -> if m=Num 0 then failwith "divide by zero" else eval' n / eval' m
   match presult with 
   | Success(e,_,_) -> eval' e
   | _ -> failwith "fix later"

let prependFormatSpecifier input : string = 
   let p = ((many1Chars (digit <|> anyOf "abcdefABCDEF")) .>> manyChars (anyOf "*-/+")) 
   let pprepender : Parser<string,unit> = match currentSystem with 
                                          | "bin" -> (many (withSkippedString (fun skp res -> "0b"+skp) p)) |>> List.reduce (+)
                                          | "oct" -> (many (withSkippedString (fun skp res -> "0o"+skp) p)) |>> List.reduce (+)
                                          | "dec" -> (many (withSkippedString (fun skp res ->       skp) p)) |>> List.reduce (+)
                                          | "hex" -> (many (withSkippedString (fun skp res -> "0x"+skp) p)) |>> List.reduce (+)

   match run pprepender input with 
   | Success(output, _, _) -> output
   | _ -> failwith "parse error" //change later 


let compute input action = 
    match action with 
    | "=" -> let res = prependFormatSpecifier input |> run opp.ExpressionParser |> eval in match currentSystem with 
                                                                                           | "bin" -> Convert.ToString(res,2)
                                                                                           | "dec" -> sprintf "%i" res
                                                                                           | "oct" -> sprintf "%o" res
                                                                                           | "hex" -> sprintf "%x" res
    | "To bin" -> let res = (prependFormatSpecifier input |> run opp.ExpressionParser |> eval) in Convert.ToString(res,2)
    | "To oct" -> prependFormatSpecifier input |> run opp.ExpressionParser |> eval |> sprintf "%o"
    | "To dec" -> prependFormatSpecifier input |> run opp.ExpressionParser |> eval |> sprintf "%i"
    | "To hex" -> prependFormatSpecifier input |> run opp.ExpressionParser |> eval |> sprintf "%x"





//////////FIX - SHOULD BE MOVED UP BUT DEPENDS ON BACKEND 
let operators = 
  [(new Button (), "=");
   (new Button (), "To bin"); (new Button (), "To oct"); (new Button (), "To dec"); (new Button (), "To hex")] 
operators |> List.iter (fun (btn, txt) -> btn.Text <- txt; btn.Size <- new Size (90,20); operatorPanel.Controls.Add btn;
                                          btn.Click.Add (fun _ -> 
                                                                  let input = inputLine.Text in 
                                                                  let action = btn.Text in
                                                                  let result = compute input action in inputLine.Text <- result))

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