AI endpoint test

1.Known category → Rice → 3 active items
2.Unknown category → Chocolate → category not found
3.Unsupported question → Weather → I don't currently support that type of business question.

Let's test three scenarios in Swagger:
Known category
How many active items are in Rice?
Expected: 3 active items

Unknown category
How many active items are in Chocolate?
Expected: category not found

Unsupported question
What is the weather today?
Expected:
I don't currently support that type of business question.
//-------------------------------------
How many active items are in Rice?  == true
How many inactive items are in Rice? == false
How many items are in Rice? == null
//---------------------------
PHASE 1
"How many active items are associated with the Electronics category?"

PHASE 2
"How many active customers are in Minneapolis, Minnesota, USA?"

PHASE 3
Multiple intents + Tool/Function Calling

PHASE 4
Embeddings + Vector DB + Document RAG

PHASE 5
Hybrid SQL RAG + Vector RAG


//---------------------------
Step 1 — Our first Phase 1C question

Let's start with:

"How many inactive items are in Rice?"

Why this is a good first question?

Because it forces our AI service to understand two pieces of business information:

Category = Rice
Status   = Inactive

Your current Phase 1B probably understands something like:

Category = Rice
Status   = Active
Operation = Count

Now we're making the intent more flexible.

//===========  Plan for Phase level
That gives us a very clean teaching progression:

Phase 1
Mock AI → SQL

Phase 1C
Better structured intent → SQL

Phase 1C.5
Real OpenAI → Structured Intent → SQL

Phase 2
Customer business questions

Phase 3
Multiple intents + tool/function calling

Phase 4
Vector RAG

Phase 5
Hybrid RAG

