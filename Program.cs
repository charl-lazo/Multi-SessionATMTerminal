using System;

class Program
{
    static void Main(string[] args)
    {
        const int CORRECT_PIN = 4821;

        double accountBalance = 1250.00;
        double dailyLimit = 500.00;
        double dailyWithdrawn = 150.00;

        bool isInternationalCard = true;
        bool isVIPCustomer = false;

        int transactionCount = 0;
        int pinAttempts = 3;
        bool isAuthenticated = false;

        double[] recentTransactions = new double[3];

        Console.WriteLine("====================================");
        Console.WriteLine("     WELCOME TO SECURE BANK ATM     ");
        Console.WriteLine("====================================");

        // PHASE 1: PIN VERIFICATION
        while (pinAttempts > 0)
        {
            Console.Write("Enter your 4-digit PIN: ");

            if (int.TryParse(Console.ReadLine(), out int enteredPin))
            {
                if (enteredPin == CORRECT_PIN)
                {
                    isAuthenticated = true;
                    Console.WriteLine("\nAccess granted!\n");
                    break;
                }
                else
                {
                    pinAttempts--;
                    Console.WriteLine($"Incorrect PIN. You have {pinAttempts} attempt(s) remaining.");
                }
            }
            else
            {
                pinAttempts--;
                Console.WriteLine(
                    $"Non-numeric entry detected. Attempts left: {pinAttempts}");
            }
        }
        if (!isAuthenticated)
        {
            Console.WriteLine("\n[CARD CONFISCATED] Maximum failed attempts reached.");
            Console.WriteLine("Please contact bank support.");
            return;
        }
        // PHASE 2: MAIN MENU LOOP
        bool sessionActive = true;

        while (sessionActive)
        {
            Console.WriteLine("\n====================================");
            Console.WriteLine("          SECURE ATM MENU");
            Console.WriteLine("====================================");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit Cash");
            Console.WriteLine("3. Withdraw Cash");
            Console.WriteLine("4. Recent Transactions (Mini-Statement)");
            Console.WriteLine("5. Exit & Eject Card");
            Console.WriteLine("====================================");
            Console.Write("Select an option (1-5): ");

            string choice = Console.ReadLine() ?? "";
            Console.WriteLine();

            // PHASE 3: MENU OPTIONS
            switch (choice)
            {
                case "1":
                    Console.WriteLine($"Current Balance: ₱{accountBalance:N2}");
                    Console.WriteLine($"Remaining Daily Withdrawal Limit: " + $"₱{Math.Max(0, dailyLimit - dailyWithdrawn):N2}");
                    break;

                case "2":
                    Console.Write("Enter deposit amount: ₱");

                    if (double.TryParse(Console.ReadLine(), out double deposit)
                        && deposit > 0
                        && deposit <= 5000.00)
                    {
                        accountBalance += deposit;

                        recentTransactions[transactionCount % recentTransactions.Length] = deposit;

                        transactionCount++;

                        Console.WriteLine($"Deposited: ₱{deposit:N2}");
                        Console.WriteLine($"New Balance: ₱{accountBalance:N2}");
                    }
                    else
                    {
                        Console.WriteLine(
                            "[X] Deposit rejected. Valid amounts: " +
                            "₱0.01 to ₱5,000.00.");
                    }

                    break;
                    case "3":
                    Console.Write("Enter withdrawal amount: ₱");

                    if (double.TryParse(Console.ReadLine(), out double withdraw)
                        && withdraw > 0)
                    {
                        // Validate possible combinations of ₱50 and ₱20 bills.
                        int remainingToDispense = (int)withdraw;
                        int bill50Count = remainingToDispense / 50;

                        bool validDenomination = withdraw == Math.Truncate(withdraw) && bill50Count >= 0 &&
                        (remainingToDispense - bill50Count * 50) >= 0;

                        if (!validDenomination)
                        {
                            Console.WriteLine("[X] Denomination Error: Machine only dispenses " + "combinations of ₱20 and ₱50 bills.");
                            break;
                        }

                        double remainingLimit = dailyLimit - dailyWithdrawn;

                        if (withdraw > remainingLimit)
                        {
                            Console.WriteLine($"[X] Declined: Amount exceeds daily limit. " + $"Remaining: ₱{Math.Max(0, remainingLimit):N2}");
                            break;
                        }

                        // Calculate applicable surcharge.
                        double surcharge = 0.00;

                        if (isInternationalCard && !isVIPCustomer)
                        {
                            surcharge += 4.50;
                        }

                        double totalRequired = withdraw + surcharge;

                        if (totalRequired > accountBalance)
                        {
                            Console.WriteLine($"[X] Declined: Total required " + $"(₱{totalRequired:N2}) exceeds balance.");
                            break;
                        }

                        // Update account after all validations pass.
                        accountBalance -= totalRequired;
                        dailyWithdrawn += withdraw;

                        recentTransactions[transactionCount % recentTransactions.Length] = -totalRequired;

                        transactionCount++;

                        Console.WriteLine($"[✓] Approved! Surcharge: ₱{surcharge:N2}");

                        // BILL DISPENSER SIMULATION
                        remainingToDispense = (int)withdraw;
                        bill50Count = remainingToDispense / 50;

                        while (bill50Count >= 0 && (remainingToDispense - bill50Count * 50) % 20 != 0)
                        {
                            bill50Count--;
                        }

                        int bill20Count = (remainingToDispense - bill50Count * 50) / 20;

                        Console.WriteLine("\nDispensing cash:");

                        for (int i = 0; i < bill50Count; i++)
                        {
                            Console.WriteLine(" -> Dispensed: ₱50.00 bill");
                        }
                        for (int i = 0; i < bill20Count; i++)
                        {
                        Console.WriteLine(" -> Dispensed: ₱20.00 bill");
                        }
                        Console.WriteLine($"Withdrawal Amount: ₱{withdraw:N2}");
                        Console.WriteLine($"Surcharge: ₱{surcharge:N2}");
                        Console.WriteLine($"Remaining Balance: ₱{accountBalance:N2}");

                        if (accountBalance <= 0)
                        {
                            Console.WriteLine("Your account balance has reached zero.");
                            sessionActive = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[X] Invalid numerical amount.");
                    }

                    break;
                    case "4":
                    Console.WriteLine("----- RECENT TRANSACTION HISTORY -----");

                    if (transactionCount == 0)
                    {
                        Console.WriteLine("No transactions logged in this session.");
                    }
                    else
                    {
                        int displayedCount = Math.Min(transactionCount, recentTransactions.Length);

                        int startIndex = transactionCount > recentTransactions.Length
                            ? transactionCount % recentTransactions.Length
                            : 0;

                        for (int i = 0; i < displayedCount; i++)
                        {
                            int index = (startIndex + i) %
                                        recentTransactions.Length;

                            double transaction = recentTransactions[index];

                            string type = transaction >= 0
                            ? "DEPOSIT"
                            : "WITHDRAW";

                            Console.WriteLine($"{i + 1}. {type}: " + $"₱{Math.Abs(transaction):N2}");
                        }

                        Console.WriteLine($"Total lifetime session actions: {transactionCount}");
                    }

                    break;
                    case "5":
                    Console.WriteLine("Ejecting card... Thank you for banking with us!");

                    sessionActive = false;
                    break;

                default:
                    Console.WriteLine("[!] Invalid selection. Please choose an option between 1 and 5.");
                    break;
            }
        }
        Console.WriteLine("\nSystem reset. Ready for next user.");
    }
}