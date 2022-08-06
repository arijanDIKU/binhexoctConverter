module Expressions 

type Expr = 
     | Num of int
     | Add of Expr*Expr
     | Sub of Expr*Expr
     | Mult of Expr*Expr
     | Div of Expr*Expr

