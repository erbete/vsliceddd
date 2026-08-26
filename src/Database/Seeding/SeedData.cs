using System;

namespace Database.Seeding;

internal static class SeedData
{
    internal sealed record AuthorSeed(Guid Id, string Name, string? Country);

    internal sealed record BookItemSeed(Guid Id, string Barcode, int AcquiredDaysAgo);

    internal sealed record BookSeed(
        Guid Id,
        string Title,
        int PublishedYear,
        Guid AuthorId,
        string? Isbn,
        BookItemSeed[] Items
    );

    internal sealed record MemberSeed(Guid Id, string Name, string Email, int MembershipDaysAgo);

    internal sealed record LoanSeed(
        Guid Id,
        Guid BookItemId,
        Guid MemberId,
        int LoanDaysAgo,
        int DueDaysAfterLoan,
        int? ReturnDaysAfterLoan
    );

    internal static readonly AuthorSeed[] Authors =
    [
        new(Guid.Parse("01960000-0000-7000-8000-000000000001"), "George Orwell", "United Kingdom"),
        new(
            Guid.Parse("01960000-0000-7000-8000-000000000002"),
            "Ursula K. Le Guin",
            "United States"
        ),
        new(Guid.Parse("01960000-0000-7000-8000-000000000003"), "J.R.R. Tolkien", "United Kingdom"),
        new(Guid.Parse("01960000-0000-7000-8000-000000000004"), "Octavia Butler", "United States"),
        new(Guid.Parse("01960000-0000-7000-8000-000000000005"), "Haruki Murakami", "Japan"),
        new(
            Guid.Parse("01960000-0000-7000-8000-000000000006"),
            "Chimamanda Ngozi Adichie",
            "Nigeria"
        ),
        new(
            Guid.Parse("01960000-0000-7000-8000-000000000007"),
            "Gabriel García Márquez",
            "Colombia"
        ),
        new(Guid.Parse("01960000-0000-7000-8000-000000000008"), "Toni Morrison", "United States"),
        new(Guid.Parse("01960000-0000-7000-8000-000000000009"), "Franz Kafka", "Czech Republic"),
        new(Guid.Parse("01960000-0000-7000-8000-00000000000a"), "N.K. Jemisin", "United States"),
    ];

    internal static readonly BookSeed[] Books =
    [
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000001"),
            "1984",
            1949,
            Authors[0].Id,
            "9780451524935",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000001"), "bk-1984-001", 120),
                new(Guid.Parse("03000000-0000-7000-8000-000000000002"), "bk-1984-002", 90),
                new(Guid.Parse("03000000-0000-7000-8000-000000000003"), "bk-1984-003", 30),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000002"),
            "The Left Hand of Darkness",
            1969,
            Authors[1].Id,
            "9780441007318",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000004"), "bk-lhod-001", 200),
                new(Guid.Parse("03000000-0000-7000-8000-000000000005"), "bk-lhod-002", 15),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000003"),
            "The Hobbit",
            1937,
            Authors[2].Id,
            "9780547928227",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000006"), "bk-hobbit-001", 300),
                new(Guid.Parse("03000000-0000-7000-8000-000000000007"), "bk-hobbit-002", 45),
                new(Guid.Parse("03000000-0000-7000-8000-000000000008"), "bk-hobbit-003", 10),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000004"),
            "Kindred",
            1979,
            Authors[3].Id,
            "9780807083697",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000009"), "bk-kindred-001", 180),
                new(Guid.Parse("03000000-0000-7000-8000-00000000000a"), "bk-kindred-002", 20),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000005"),
            "Norwegian Wood",
            1987,
            Authors[4].Id,
            "9780375704024",
            [
                new(Guid.Parse("03000000-0000-7000-8000-00000000000b"), "bk-nwood-001", 100),
                new(Guid.Parse("03000000-0000-7000-8000-00000000000c"), "bk-nwood-002", 60),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000006"),
            "Half of a Yellow Sun",
            2006,
            Authors[5].Id,
            "9781400095209",
            [
                new(Guid.Parse("03000000-0000-7000-8000-00000000000d"), "bk-hoays-001", 80),
                new(Guid.Parse("03000000-0000-7000-8000-00000000000e"), "bk-hoays-002", 25),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000007"),
            "One Hundred Years of Solitude",
            1967,
            Authors[6].Id,
            "9780060883287",
            [
                new(Guid.Parse("03000000-0000-7000-8000-00000000000f"), "bk-ohys-001", 250),
                new(Guid.Parse("03000000-0000-7000-8000-000000000010"), "bk-ohys-002", 40),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000008"),
            "Beloved",
            1987,
            Authors[7].Id,
            "9781400033416",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000011"), "bk-beloved-001", 150),
                new(Guid.Parse("03000000-0000-7000-8000-000000000012"), "bk-beloved-002", 35),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000009"),
            "The Metamorphosis",
            1915,
            Authors[8].Id,
            null,
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000013"), "bk-metam-001", 70),
                new(Guid.Parse("03000000-0000-7000-8000-000000000014"), "bk-metam-002", 5),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-00000000000a"),
            "The Fifth Season",
            2015,
            Authors[9].Id,
            "9780316229296",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000015"), "bk-tfs-001", 95),
                new(Guid.Parse("03000000-0000-7000-8000-000000000016"), "bk-tfs-002", 12),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-00000000000b"),
            "Animal Farm",
            1945,
            Authors[0].Id,
            "9780451526342",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000017"), "bk-afarm-001", 110),
                new(Guid.Parse("03000000-0000-7000-8000-000000000018"), "bk-afarm-002", 18),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-00000000000c"),
            "The Dispossessed",
            1974,
            Authors[1].Id,
            "9780060512750",
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000019"), "bk-dispos-001", 130),
                new(Guid.Parse("03000000-0000-7000-8000-00000000001a"), "bk-dispos-002", 22),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-00000000000d"),
            "The Lord of the Rings",
            1954,
            Authors[2].Id,
            null,
            [
                new(Guid.Parse("03000000-0000-7000-8000-00000000001b"), "bk-lotr-001", 400),
                new(Guid.Parse("03000000-0000-7000-8000-00000000001c"), "bk-lotr-002", 50),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-00000000000e"),
            "Parable of the Sower",
            1993,
            Authors[3].Id,
            "9780446675505",
            [
                new(Guid.Parse("03000000-0000-7000-8000-00000000001d"), "bk-pots-001", 85),
                new(Guid.Parse("03000000-0000-7000-8000-00000000001e"), "bk-pots-002", 28),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-00000000000f"),
            "Kafka on the Shore",
            2002,
            Authors[4].Id,
            "9781400079278",
            [
                new(Guid.Parse("03000000-0000-7000-8000-00000000001f"), "bk-kots-001", 75),
                new(Guid.Parse("03000000-0000-7000-8000-000000000020"), "bk-kots-002", 8),
            ]
        ),
        new(
            Guid.Parse("02000000-0000-7000-8000-000000000010"),
            "Americanah",
            2013,
            Authors[5].Id,
            null,
            [
                new(Guid.Parse("03000000-0000-7000-8000-000000000021"), "bk-amer-001", 65),
                new(Guid.Parse("03000000-0000-7000-8000-000000000022"), "bk-amer-002", 14),
            ]
        ),
    ];

    internal static readonly MemberSeed[] Members =
    [
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000001"),
            "Alice Johnson",
            "alice.johnson@example.com",
            400
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000002"),
            "Bob Smith",
            "bob.smith@example.com",
            350
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000003"),
            "Carol Davis",
            "carol.davis@example.com",
            300
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000004"),
            "David Wilson",
            "david.wilson@example.com",
            200
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000005"),
            "Emma Brown",
            "emma.brown@example.com",
            150
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000006"),
            "Frank Miller",
            "frank.miller@example.com",
            100
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000007"),
            "Grace Lee",
            "grace.lee@example.com",
            80
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000008"),
            "Henry Taylor",
            "henry.taylor@example.com",
            60
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-000000000009"),
            "Iris Chen",
            "iris.chen@example.com",
            30
        ),
        new(
            Guid.Parse("04000000-0000-7000-8000-00000000000a"),
            "Jack Anderson",
            "jack.anderson@example.com",
            10
        ),
    ];

    internal static readonly LoanSeed[] Loans =
    [
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000001"),
            Guid.Parse("03000000-0000-7000-8000-000000000001"),
            Members[0].Id,
            5,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000002"),
            Guid.Parse("03000000-0000-7000-8000-000000000004"),
            Members[1].Id,
            10,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000003"),
            Guid.Parse("03000000-0000-7000-8000-000000000006"),
            Members[2].Id,
            3,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000004"),
            Guid.Parse("03000000-0000-7000-8000-000000000009"),
            Members[3].Id,
            7,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000005"),
            Guid.Parse("03000000-0000-7000-8000-00000000000b"),
            Members[4].Id,
            2,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000006"),
            Guid.Parse("03000000-0000-7000-8000-00000000000d"),
            Members[5].Id,
            1,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000007"),
            Guid.Parse("03000000-0000-7000-8000-00000000000f"),
            Members[6].Id,
            30,
            14,
            null
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000008"),
            Guid.Parse("03000000-0000-7000-8000-000000000002"),
            Members[0].Id,
            40,
            14,
            10
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-000000000009"),
            Guid.Parse("03000000-0000-7000-8000-000000000005"),
            Members[1].Id,
            50,
            14,
            7
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-00000000000a"),
            Guid.Parse("03000000-0000-7000-8000-000000000007"),
            Members[2].Id,
            35,
            14,
            12
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-00000000000b"),
            Guid.Parse("03000000-0000-7000-8000-00000000000a"),
            Members[3].Id,
            60,
            14,
            5
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-00000000000c"),
            Guid.Parse("03000000-0000-7000-8000-00000000000c"),
            Members[4].Id,
            45,
            14,
            8
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-00000000000d"),
            Guid.Parse("03000000-0000-7000-8000-000000000011"),
            Members[5].Id,
            25,
            14,
            10
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-00000000000e"),
            Guid.Parse("03000000-0000-7000-8000-000000000013"),
            Members[6].Id,
            20,
            14,
            6
        ),
        new(
            Guid.Parse("05000000-0000-7000-8000-00000000000f"),
            Guid.Parse("03000000-0000-7000-8000-000000000015"),
            Members[7].Id,
            15,
            14,
            9
        ),
    ];
}
