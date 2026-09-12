Ollama.com install free LLM
Pllama --version 
ollama pull llama3.2

Single Intent and Multi Intent test questions
Single-intent (sanity check each function fires correctly):

"How many customers do we have in Chennai?"
"How many vendors are there in Tamil Nadu?"
"How many items are measured in KG?"
"Give me the customer count by city for all cities."
"Show me vendor counts for every state."
"Break down item counts by unit of measure."

Multi-intent (combines 2+ plugins in one message — this is the real test):
7. "How many customers are in Chennai and how many vendors are in Karnataka?"
8. "Give me the total customer count and item count for the Rice category."
9. "How many active vendors do we have, and how many items are measured in LTR?"
10. "What's the customer count in Mumbai, vendor count in Gujarat, and item count in PCS?" (tests all three plugins at once)

Not-found / -1 handling (confirms the sentinel logic works end-to-end):
11. "How many customers are in Atlantis?" (fake city — should say it doesn't exist, not report 0)
12. "How many vendors are in Wakanda?"
13. "How many items use the UOM 'XYZ'?"
14. "How many items are in the 'Electronics' category?" (if that category doesn't exist in your data)

//====================================
Run Model in the command prompt with question
PS C:\Users\arulg> ollama run qwen2.5:7b-instruct
⠸ >>> What is the capital of France?
The capital of France is Paris.
Windows PowerShell
Copyright (C) Microsoft Corporation. All rights reserved.

Try the new cross-platform PowerShell https://aka.ms/pscore6

PS C:\Users\arulg> ollama list
NAME                   ID              SIZE      MODIFIED
qwen2.5:7b-instruct    845dbda0ea48    4.7 GB    2 days ago
llama3.2:latest        a80c4f17acd5    2.0 GB    5 days ago
PS C:\Users\arulg> ollama run qwen2.5:7b-instruct
>>> expect with exactly: Ollama is working
It seems you might have intended to say "Ollama is working" but want to ensure it's exactly as stated. If you
meant to compare or check if something is exactly "Ollama is working," then your statement is clear. However, if
you're looking for a specific context or need assistance with something related, please provide more details. For
example:

- Are you checking the status of a system named Ollama?
- Do you need help with a task related to Ollama?
- Is there a specific format or context you need this in?

Feel free to elaborate, and I'll be happy to help!

PS C:\Users\arulg> $body = @{
>>     model = "qwen2.5:7b-instruct"
>>     messages = @(
>>         @{
>>             role = "user"
>>             content = "Respond with exactly: Ollama API is working."
>>         }
>>     )
>>     stream = $false
>> } | ConvertTo-Json -Depth 10
>>
>> Invoke-RestMethod `
>>     -Uri "http://localhost:11434/v1/chat/completions" `
>>     -Method Post `
>>     -ContentType "application/json" `
>>     -Body $body


id                 : chatcmpl-694
object             : chat.completion
created            : 1789238859
model              : qwen2.5:7b-instruct
system_fingerprint : fp_ollama
choices            : {@{index=0; message=; finish_reason=stop}}
usage              : @{prompt_tokens=40; prompt_tokens_details=; completion_tokens=8; total_tokens=48}



PS C:\Users\arulg> $response = Invoke-RestMethod `
>>     -Uri "http://localhost:11434/v1/chat/completions" `
>>     -Method Post `
>>     -ContentType "application/json" `
>>     -Body $body
>>
>> $response.choices[0].message.content
Ollama API is working.
//=======================
Our next move: bypass Semantic Kernel for one test

I don't want to change your application architecture yet. Let's directly test whether Ollama/Qwen accepts an OpenAI tool definition.

Run this in PowerShell:

$body = @{
    model = "qwen2.5:7b-instruct"
    messages = @(
        @{
            role = "user"
            content = "How many customers are there?"
        }
    )
    tools = @(
        @{
            type = "function"
            function = @{
                name = "get_customer_count"
                description = "Gets the count of customers."
                parameters = @{
                    type = "object"
                    properties = @{
                        isActive = @{
                            type = "boolean"
                            description = "true for active customers, false for inactive customers"
                        }
                    }
                }
            }
        }
    )
    stream = $false
} | ConvertTo-Json -Depth 20

$sw = [System.Diagnostics.Stopwatch]::StartNew()

$response = Invoke-RestMethod `
    -Uri "http://localhost:11434/v1/chat/completions" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body

$sw.Stop()

Write-Host "Elapsed: $($sw.Elapsed.TotalSeconds) seconds"
$response.choices[0].message | Format-List *
What we're looking for

If Qwen supports the tool request correctly, we should see something resembling:

tool_calls : ...

with a function name such as:

get_customer_count
//====
Next test — inspect the actual tool call

Run:
$response.choices[0].message.tool_calls[0].function | Format-List *
Then also run:

$response.choices[0].message.tool_calls[0] | ConvertTo-Json -Depth 20
PS C:\Users\arulg> $response.choices[0].message.tool_calls[0].function | Format-List *


name      : get_customer_count
arguments : {"isActive":true}



PS C:\Users\arulg> $response.choices[0].message.tool_calls[0] | ConvertTo-Json -Depth 20
{
    "id":  "call_1dhlckdx",
    "index":  0,
    "type":  "function",
    "function":  {
                     "name":  "get_customer_count",
                     "arguments":  "{\"isActive\":true}"
                 }


                 Excellent, my friend. This is the breakthrough we needed. ✅

Your direct Ollama test proves that Qwen is generating a valid OpenAI-compatible function/tool call:

{
  "id": "call_1dhlckdx",
  "index": 0,
  "type": "function",
  "function": {
    "name": "get_customer_count",
    "arguments": "{\"isActive\":true}"
  }
}

And even better:

Function name: get_customer_count ✅
Argument name: isActive ✅
Argument value: true ✅
Ollama returns the tool call successfully ✅
Direct call takes ~16 seconds ✅

So CustomerPlugin itself is not the problem, and Ollama/Qwen tool calling itself is not the problem.


What I need from you

Send me one of these two things:

The response from /api/ai/test-sk-normal and the contents of ollama-http-debug.txt, or
If ollama-http-debug.txt doesn't exist, just tell me:

ollama-http-debug.txt does not exist

//=============================================================

Ollama rules
var systemPrompt =
    $$"""
    You are an intent-extraction engine for an Invoice Management System.
    Known categories: {{categoryNames}}.

    Given a user's question, respond with ONLY a raw JSON object
    (no markdown fences, no explanation, no extra text) in exactly this shape:

    {
      "intent": "CategoryItemCount" or "Unknown",
      "categoryName": "<one of the known categories, or null>",
      "categoryActiveOnly": true,
      "itemActiveOnly": true, false, or null
    }

    Rules:
    - Use "CategoryItemCount" whenever the question asks how many items exist in a category.
    - Set itemActiveOnly to true ONLY if the question asks about "active" items.
    - Set itemActiveOnly to false ONLY if the question asks about "inactive" items.
      Note: "inactive" is a DIFFERENT word from "active" even though it contains those
      letters - read the whole word carefully.
    - Set itemActiveOnly to null only if the question does not mention active/inactive at all.
    - Match categoryName to the closest known category name, correcting for
      typos, casing, or plural/singular differences (e.g. "rices" -> "Rice").
    - If the question is unrelated to category item counts, or you cannot
      match a known category, return intent "Unknown" with the other fields null.
    - Never invent a category name that isn't in the known list.

    Examples:
    Q: "How many active items are in Rice?"
    A: {"intent":"CategoryItemCount","categoryName":"Rice","categoryActiveOnly":true,"itemActiveOnly":true}

    Q: "How many inactive items are in Rice?"
    A: {"intent":"CategoryItemCount","categoryName":"Rice","categoryActiveOnly":true,"itemActiveOnly":false}

    Q: "How many items are in Rice?"
    A: {"intent":"CategoryItemCount","categoryName":"Rice","categoryActiveOnly":true,"itemActiveOnly":null}
    """;

    AIMulti Test 
    Test it with things like:

"How many active customers are there?" (single, new entity)
"How many active items are in Rice and how many vendors are inactive?"
3. Test matrix for both endpoints

Run these against /api/ai/ask and /api/ai/ask-multi:

#	Question	Endpoint	Expects
1	"How many active items are in Rice?"	ask	itemActiveOnly=true
2	"How many inactive items are in Rice?"	ask	itemActiveOnly=false
3	"How many items are in Rice?"	ask	itemActiveOnly=null
4	"How many active customers are there?"	ask-multi	single entity
5	"How many inactive vendors are there?"	ask-multi	single entity
6	"How many users are there?"	ask-multi	null filter
7	"How many active items are in Rice and how many inactive vendors are there?"	ask-multi	mixed active/inactive, 2 entities
8	"How many active customers, inactive vendors, and total users are there?"	ask-multi	3 entities, 3 different filter states

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
//======  Feature Plan 
Phase 3 — Multiple Intents + Tool / Function Calling

This is where things become much more impressive:

"How many active customers are in Minneapolis and how many active items are in Soap?"

The LLM needs to recognize multiple business intents, then your application decides which tools/functions to execute.

Something like:

User Natural Language
        ↓
       LLM
        ↓
Multiple Structured Intents
        ↓
 ┌──────────────┬──────────────┐
 │ Customer Tool│  Item Tool   │
 └──────────────┴──────────────┘
        ↓
     SQL Server
        ↓
      Results
        ↓
       LLM
        ↓
Natural Language Answer

That will be a major step up from Phase 1.

Then Phase 4 becomes the RAG story

Phase 4 — Embeddings + Vector DB + Document RAG

Now we introduce information that isn't naturally represented as relational business data.

For example:

"What is our return policy for damaged items?"

or:

"What are the payment terms for this customer?"

Then:

Document
   ↓
Chunking
   ↓
Embeddings
   ↓
Vector Database
   ↓
Similarity Search
   ↓
Relevant Context
   ↓
LLM
   ↓
Natural Language Answer

This gives your students a very clear understanding of why RAG exists.

Phase 5 is where your project becomes really interesting

Hybrid SQL RAG + Vector RAG

This is the architecture I'd ultimately want you to demonstrate.

For example:

"What are the outstanding invoices for ABC Customer, and what are their payment terms according to the customer agreement?"

Now the system needs two different knowledge sources:

                    User Question
                          ↓
                         LLM
                          ↓
                 Intent / Tool Router
                    ↙             ↘
             SQL Tool          Vector RAG
                 ↓                 ↓
          Invoice/Customer     Agreement
              Database          Documents
                    ↘             ↙
                       Results
                          ↓
                         LLM
                          ↓
              Natural Language Answer

That is a very strong practical AI architecture for an Invoice Management System.

//==============
That way your students can see the evolution:

Phase 1
Single Intent
      ↓
Phase 2
Customer Intent
      ↓
Phase 3
Multiple Intents + Tools
      ↓
Phase 4
Vector RAG
      ↓
Phase 5
Hybrid SQL + Vector RAG

And later we replace:

MockAIProvider

with the real OpenAI LLM, without throwing away the business/tool architecture we've built.

//==== Customer CRUD Implementation for Phase 2 Multiple Intent ==============
Absolutely, my friend. 👍 I agree with this change completely.

This is actually a better learning sequence for you:

Customer API CRUD
      ↓
EF Stored Procedures / Repository
      ↓
Phase 2 — Customer AI
      ↓
Phase 3 — Multiple Intents + Tool Calling
      ↓
Phase 4 — Embeddings + Vector RAG
      ↓
Phase 5 — Hybrid SQL + Vector RAG

Finishing the Customer CRUD first will make the AI work much easier to understand because you'll already know exactly how the customer data flows through your application.

And for interviews, you'll be able to explain the evolution naturally:

"I first built the traditional Invoice Management APIs using EF/repository patterns and SQL Server. Then I added an AI layer that converts natural-language questions into structured intents and routes those intents to controlled application logic. From there, I extended it to multiple intents and tool calling, followed by RAG and eventually hybrid SQL + vector retrieval."

That's a much stronger story than trying to build all the AI pieces at once.

For now, let's focus on Customer CRUD, get the EF/SP/repository/service/controller flow solid, test it, commit it, and then come back to Phase 2 — Multiple Intent Customer AI.

You're making the right call. 💪

Enjoy the Customer implementation, and when you come back, we'll pick up exactly from there.


Valavan gitbub URl https://github.com/valavan05/Invoice_.NetCoreApi.git 

//==========  Next work by AI =================
Your next logical step is the AI work we discussed:

Phase 1
AI Business Query Foundation
        ✅ COMPLETED

        ↓

Customer + Vendor CRUD
        ✅ COMPLETED

        ↓

Phase 2
Multiple Business Intents
        ⏭️ NEXT

        ↓

Phase 3
Tool / Function Calling
        ↓

Phase 4
Embeddings + Vector DB + RAG
        ↓

Phase 5
Hybrid SQL RAG + Vector RAG

And importantly, we won't throw away what you built in Phase 1. We'll extend the existing AIService → Intent → Business Logic → Repository/SP architecture rather than starting over.

Very good progress today, my friend. This is becoming a real portfolio/interview project rather than just isolated CRUD exercises.
//====================
🚀 For next week's class

I would recommend that we make the next step feel like a real AI feature, not just another coding exercise.

A possible progression:

Phase 1 — Single Intent ✅ Completed
Category count
Category lookup
Category-related natural-language questions
Phase 2 — Multiple Intent
One question can require multiple pieces of information.
Example:
“How many active categories do we have and what is the total number of items?”
Phase 3 — AI + Multiple Tables
Category + Itemmaster
Customer + Orders
Vendor + Items
Phase 4 — AI Understanding Business Language
User doesn't need to know database terminology.
“Show me our rice-related products.”
AI determines the intent and parameters.
Phase 5 — AI + Angular UI
Add an AI Assistant screen to the Invoice Management System.
Students type a question.
Angular → API → AI → database/business service → response.
Phase 6 — AI Actions
Eventually move from AI answering questions to AI assisting with operations, with proper confirmation and authorization.
Example: “Find inactive categories.”
Later: “Create a new category called…” → confirmation → API action.

That progression can turn their existing Invoice Management System into a genuinely AI-enabled enterprise application.

And my friend, you should absolutely be proud of this. You are not just teaching them syntax or interview questions anymore—you are showing them how modern developers integrate AI into real .NET applications.

Phase 1 is officially a success. 🏆

Next weekend, we can start Phase 2: Multiple-Intent AI, step-by-step, using the same architecture they already understand.
