# Login
{
  "email": "admin@development.local",
  "password": "DevAdmin2026!StrongPassword",
  "tenantId": "229051b8-dfbd-4286-8de0-7bb9849ffc73"
}
# Company Create
{
  "name": "Demo Trading LLC",
  "code": "DEMO",
  "legalName": "Demo Trading Limited Liability Company",
  "countryCode": "AE",
  "baseCurrencyCode": "AED"
}
{
  "id": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "name": "Demo Trading LLC",
  "code": "DEMO",
  "legalName": "Demo Trading Limited Liability Company",
  "countryCode": "AE",
  "baseCurrencyCode": "AED",
  "isActive": true
}
# Accounting Book
{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "name": "Main Financial Book",
  "code": "MAIN",
  "accountingBasis": "Accrual",
  "currencyCode": "AED"
}
{
  "id": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "name": "Main Financial Book",
  "code": "MAIN",
  "accountingBasis": "Accrual",
  "currencyCode": "AED",
  "isActive": true,
  "createdAtUtc": "2026-11-26T09:05:29.0257904Z"
}
# COA Main
{
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "name": "Main Chart of Accounts",
  "code": "COA-MAIN",
  "description": "Primary chart of accounts for Demo Trading LLC"
}
{
  "id": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "name": "Main Chart of Accounts",
  "code": "COA-MAIN",
  "description": "Primary chart of accounts for Demo Trading LLC",
  "isActive": true,
  "createdAtUtc": "2026-11-26T09:06:52.3122197Z"
}
# Account
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "1000",
  "name": "Cash and Cash Equivalents",
  "accountType": 1,
  "accountCategory": 1,
  "level": 1,
  "isPostingAllowed": false
}
{
  "id": "9a8221c2-9abd-4f1e-b7ca-7a2f60bfdb34",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "1000",
  "name": "Cash and Cash Equivalents",
  "accountType": 1,
  "accountCategory": 1,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": false,
  "isActive": true,
  "level": 1,
  "createdAtUtc": "2026-11-26T09:08:27.39425Z"
}
# 1001
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "9a8221c2-9abd-4f1e-b7ca-7a2f60bfdb34",
  "code": "1001",
  "name": "Cash on Hand",
  "accountType": 1,
  "accountCategory": 1,
  "level": 2,
  "isPostingAllowed": true
}
{
  "id": "6c910eab-b841-41b5-8b58-450a951ac94e",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "9a8221c2-9abd-4f1e-b7ca-7a2f60bfdb34",
  "code": "1001",
  "name": "Cash on Hand",
  "accountType": 1,
  "accountCategory": 1,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": true,
  "isActive": true,
  "level": 2,
  "createdAtUtc": "2026-11-26T09:11:00.673195Z"
}
# PUT /api/accounts/6c910eab-b841-41b5-8b58-450a951ac94e
{
  "cashFlowCategory": 1,
  "isCashOrCashEquivalent": true
}
{
  "id": "6c910eab-b841-41b5-8b58-450a951ac94e",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "9a8221c2-9abd-4f1e-b7ca-7a2f60bfdb34",
  "code": "1001",
  "name": "Cash on Hand",
  "accountType": 1,
  "accountCategory": 1,
  "cashFlowCategory": 1,
  "isCashOrCashEquivalent": true,
  "isPostingAllowed": true,
  "isActive": true,
  "level": 2,
  "createdAtUtc": "2026-11-26T09:11:00.673195Z"
}
# 3000
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "3000",
  "name": "Equity",
  "accountType": 3,
  "accountCategory": 5,
  "level": 1,
  "isPostingAllowed": false
}
{
  "id": "5041b501-99dc-42fd-b981-b0e15fd9d7f9",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "3000",
  "name": "Equity",
  "accountType": 3,
  "accountCategory": 5,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": false,
  "isActive": true,
  "level": 1,
  "createdAtUtc": "2026-11-26T12:50:38.744478Z"
}
# 3100
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "5041b501-99dc-42fd-b981-b0e15fd9d7f9",
  "code": "3100",
  "name": "Retained Earnings",
  "accountType": 3,
  "accountCategory": 5,
  "level": 2,
  "isPostingAllowed": true
}
{
  "id": "a29b58b2-54f5-4748-866f-582e5aef8d58",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "5041b501-99dc-42fd-b981-b0e15fd9d7f9",
  "code": "3100",
  "name": "Retained Earnings",
  "accountType": 3,
  "accountCategory": 5,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": true,
  "isActive": true,
  "level": 2,
  "createdAtUtc": "2026-11-26T12:52:19.81016Z"
}
# POST /api/general-ledger/fiscal-periods
{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "name": "FY 2026",
  "startDate": "2026-01-01",
  "endDate": "2026-12-31"
}
{
  "id": "d8b7acbc-9616-4a27-a5dd-508cbaa483bb",
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "name": "FY 2026",
  "startDate": "2026-01-01",
  "endDate": "2026-12-31",
  "status": 1
}
# 4000
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "4000",
  "name": "Revenue",
  "accountType": 4,
  "accountCategory": 6,
  "level": 1,
  "isPostingAllowed": false
}
{
  "id": "cbe0f234-6c36-46f0-b73a-6f011fca0f48",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "4000",
  "name": "Revenue",
  "accountType": 4,
  "accountCategory": 6,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": false,
  "isActive": true,
  "level": 1,
  "createdAtUtc": "2026-11-26T13:26:19.030574Z"
}
# 4100
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "cbe0f234-6c36-46f0-b73a-6f011fca0f48",
  "code": "4100",
  "name": "Sales Revenue",
  "accountType": 4,
  "accountCategory": 6,
  "level": 2,
  "isPostingAllowed": true
}
{
  "id": "241c8e09-f2fc-4375-a4fa-172364139bb9",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "cbe0f234-6c36-46f0-b73a-6f011fca0f48",
  "code": "4100",
  "name": "Sales Revenue",
  "accountType": 4,
  "accountCategory": 6,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": true,
  "isActive": true,
  "level": 2,
  "createdAtUtc": "2026-11-26T13:27:39.851912Z"
}
# 5000
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "5000",
  "name": "Expenses",
  "accountType": 5,
  "accountCategory": 9,
  "level": 1,
  "isPostingAllowed": false
}
{
  "id": "3093f477-911f-459c-b930-cd41571534dc",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": null,
  "code": "5000",
  "name": "Expenses",
  "accountType": 5,
  "accountCategory": 9,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": false,
  "isActive": true,
  "level": 1,
  "createdAtUtc": "2026-11-26T13:31:32.752119Z"
}
# 5100
{
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "3093f477-911f-459c-b930-cd41571534dc",
  "code": "5100",
  "name": "Operating Expenses",
  "accountType": 5,
  "accountCategory": 9,
  "level": 2,
  "isPostingAllowed": true
}
{
  "id": "8839f829-7f51-4e0b-bc0c-a76f57e0ad6b",
  "chartOfAccountsId": "e7f4aad4-4e32-44ba-9d79-25644a9084cc",
  "parentAccountId": "3093f477-911f-459c-b930-cd41571534dc",
  "code": "5100",
  "name": "Operating Expenses",
  "accountType": 5,
  "accountCategory": 9,
  "cashFlowCategory": 4,
  "isCashOrCashEquivalent": false,
  "isPostingAllowed": true,
  "isActive": true,
  "level": 2,
  "createdAtUtc": "2026-11-26T13:34:31.875985Z"
}
# 1000 AED - Create Journal
{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "journalNumber": "TEST-REV-2026-001",
  "description": "Test sales revenue transaction",
  "journalDate": "2026-11-26",
  "journalType": 2,
  "journalSource": 1,
  "lines": [
    {
      "accountId": "6c910eab-b841-41b5-8b58-450a951ac94e",
      "description": "Cash received from sales",
      "debit": 1000,
      "credit": 0,
      "currencyCode": "AED",
      "cashFlowActivity": 1
    },
    {
      "accountId": "241c8e09-f2fc-4375-a4fa-172364139bb9",
      "description": "Sales revenue earned",
      "debit": 0,
      "credit": 1000,
      "currencyCode": "AED",
      "cashFlowActivity": 1
    }
  ]
}
{
  "id": "b591795d-c925-4b19-ad46-81ecaa3c1e89"
}
# 600 AED - Journal Create
{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "journalNumber": "TEST-EXP-2026-001",
  "description": "Test operating expense payment",
  "journalDate": "2026-11-26",
  "journalType": 1,
  "journalSource": 1,
  "lines": [
    {
      "accountId": "8839f829-7f51-4e0b-bc0c-a76f57e0ad6b",
      "description": "Operating expenses paid",
      "debit": 600,
      "credit": 0,
      "currencyCode": "AED",
      "cashFlowActivity": 1
    },
    {
      "accountId": "6c910eab-b841-41b5-8b58-450a951ac94e",
      "description": "Cash paid for operating expenses",
      "debit": 0,
      "credit": 600,
      "currencyCode": "AED",
      "cashFlowActivity": 1
    }
  ]
}
{
  "id": "3747eeb3-648a-48bf-9f86-b7a31d483ead"
}

# /api/general-ledger/reports/trial-balance?companyId=f5748c06-d9a8-48ab-97bd-0b22d2718ef0&accountingBookId=a58e9953-c92c-4678-bcfd-d427aac0f775&fiscalPeriodId=d8b7acbc-9616-4a27-a5dd-508cbaa483bb&currencyCode=AED

{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "fiscalPeriodId": "d8b7acbc-9616-4a27-a5dd-508cbaa483bb",
  "currencyCode": "AED",
  "lines": [
    {
      "accountId": "241c8e09-f2fc-4375-a4fa-172364139bb9",
      "accountCode": "4100",
      "accountName": "Sales Revenue",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 0,
      "periodCredit": 1000,
      "closingDebit": 0,
      "closingCredit": 1000,
      "currencyCode": "AED",
      "accountType": "Revenue",
      "accountCategory": "OperatingRevenue"
    },
    {
      "accountId": "6c910eab-b841-41b5-8b58-450a951ac94e",
      "accountCode": "1001",
      "accountName": "Cash on Hand",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 1000,
      "periodCredit": 600,
      "closingDebit": 400,
      "closingCredit": 0,
      "currencyCode": "AED",
      "accountType": "Asset",
      "accountCategory": "CurrentAsset"
    },
    {
      "accountId": "8839f829-7f51-4e0b-bc0c-a76f57e0ad6b",
      "accountCode": "5100",
      "accountName": "Operating Expenses",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 600,
      "periodCredit": 0,
      "closingDebit": 600,
      "closingCredit": 0,
      "currencyCode": "AED",
      "accountType": "Expense",
      "accountCategory": "OperatingExpense"
    }
  ],
  "totalOpeningDebit": 0,
  "totalOpeningCredit": 0,
  "totalPeriodDebit": 1600,
  "totalPeriodCredit": 1600,
  "totalClosingDebit": 1000,
  "totalClosingCredit": 1000,
  "isOpeningBalanced": true,
  "isPeriodBalanced": true,
  "isClosingBalanced": true
}

# Year-End-Closing
fiscal_id: d8b7acbc-9616-4a27-a5dd-508cbaa483bb
retrival_id: a29b58b2-54f5-4748-866f-582e5aef8d58

{
  "message": "Year-end closing journals generated successfully.",
  "fiscalPeriodId": "d8b7acbc-9616-4a27-a5dd-508cbaa483bb",
  "journalCount": 1,
  "journalEntryIds": [
    "fbdc120b-0e10-4b92-9262-2671c8eba3b3"
  ],
  "status": "Draft"
}

# GET /api/general-ledger/reports/trial-balance
Use:
- companyId: f5748c06-d9a8-48ab-97bd-0b22d2718ef0
- accountingBookId: a58e9953-c92c-4678-bcfd-d427aac0f775
- fiscalPeriodId: d8b7acbc-9616-4a27-a5dd-508cbaa483bb
- currencyCode: AED

{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "fiscalPeriodId": "d8b7acbc-9616-4a27-a5dd-508cbaa483bb",
  "currencyCode": "AED",
  "lines": [
    {
      "accountId": "241c8e09-f2fc-4375-a4fa-172364139bb9",
      "accountCode": "4100",
      "accountName": "Sales Revenue",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 1000,
      "periodCredit": 1000,
      "closingDebit": 0,
      "closingCredit": 0,
      "currencyCode": "AED",
      "accountType": "Revenue",
      "accountCategory": "OperatingRevenue"
    },
    {
      "accountId": "6c910eab-b841-41b5-8b58-450a951ac94e",
      "accountCode": "1001",
      "accountName": "Cash on Hand",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 1000,
      "periodCredit": 600,
      "closingDebit": 400,
      "closingCredit": 0,
      "currencyCode": "AED",
      "accountType": "Asset",
      "accountCategory": "CurrentAsset"
    },
    {
      "accountId": "8839f829-7f51-4e0b-bc0c-a76f57e0ad6b",
      "accountCode": "5100",
      "accountName": "Operating Expenses",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 600,
      "periodCredit": 600,
      "closingDebit": 0,
      "closingCredit": 0,
      "currencyCode": "AED",
      "accountType": "Expense",
      "accountCategory": "OperatingExpense"
    },
    {
      "accountId": "a29b58b2-54f5-4748-866f-582e5aef8d58",
      "accountCode": "3100",
      "accountName": "Retained Earnings",
      "openingDebit": 0,
      "openingCredit": 0,
      "periodDebit": 0,
      "periodCredit": 400,
      "closingDebit": 0,
      "closingCredit": 400,
      "currencyCode": "AED",
      "accountType": "Equity",
      "accountCategory": "Equity"
    }
  ],
  "totalOpeningDebit": 0,
  "totalOpeningCredit": 0,
  "totalPeriodDebit": 2600,
  "totalPeriodCredit": 2600,
  "totalClosingDebit": 400,
  "totalClosingCredit": 400,
  "isOpeningBalanced": true,
  "isPeriodBalanced": true,
  "isClosingBalanced": true
}

# POST /api/general-ledger/fiscal-periods/{id}/actions/{operation}

Use:
- id: d8b7acbc-9616-4a27-a5dd-508cbaa483bb
- operation: close

204

# POST /api/general-ledger/fiscal-periods
{
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "name": "FY 2027",
  "startDate": "2027-01-01",
  "endDate": "2027-12-31"
}
{
  "id": "1b7d34ee-1f84-4e1c-9541-ffb0d6110249",
  "companyId": "f5748c06-d9a8-48ab-97bd-0b22d2718ef0",
  "accountingBookId": "a58e9953-c92c-4678-bcfd-d427aac0f775",
  "name": "FY 2027",
  "startDate": "2027-01-01",
  "endDate": "2027-12-31",
  "status": 1
}

# POST /api/general-ledger/fiscal-periods/{id}/carry-forward
Use the FY 2027 ID:

id: 1b7d34ee-1f84-4e1c-9541-ffb0d6110249

{
  "message": "Opening balances carried forward successfully.",
  "targetFiscalPeriodId": "1b7d34ee-1f84-4e1c-9541-ffb0d6110249",
  "balancesCarriedForward": 2
}

