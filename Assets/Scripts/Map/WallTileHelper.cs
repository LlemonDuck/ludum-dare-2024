using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallTileHelper {

    public static bool IsCornerNorthEast(int input) {
        return CheckBitMask(input, 0b00000100, 0b01010101);
    }

    public static bool IsCornerNorthEastOpen(int input) {
        return CheckBitMask(input, 0b10100000, 0b11110001);
    }

    public static bool IsCornerNorthWest(int input) {
        return CheckBitMask(input, 0b00010000, 0b01010101);
    }

    public static bool IsCornerNorthWestOpen(int input) {
        return CheckBitMask(input, 0b10000010, 0b11000111);
    }

    public static bool IsCornerSouthWest(int input) {
        return CheckBitMask(input, 0b01000000, 0b01010101);
    }

    public static bool IsCornerSouthWestOpen(int input) {
        return CheckBitMask(input, 0b00001010, 0b00011111);
    }
    
    public static bool IsCornerSouthEast(int input) {
        return CheckBitMask(input, 0b00000001, 0b01010101);
    }

    public static bool IsCornerSouthEastOpen(int input) {
        return CheckBitMask(input, 0b00101000, 0b01111100);
    }

    public static bool isWallWest(int input) {
        return CheckBitMask(input, 0b0100, 0b1111);
    }

    public static bool isWallEast(int input) {
        return CheckBitMask(input, 0b0001, 0b1111);
    }

    public static bool iswallSouth(int input) {
        return CheckBitMask(input, 0b1000, 0b1111);
    }

    public static bool iswallNorth(int input) {
        return CheckBitMask(input, 0b0010, 0b1111);
    }   

    // TODO: fix me
    public static bool IsWallAll(int input) {
        return CheckBitMask(input, 0b00000000, 0b00110110);
    }

    private static bool CheckBitMask(int input, byte andBm, byte orBm) {
        return (input | orBm) == orBm && (input & andBm) == andBm;
    }

    public static HashSet<int> wallNorth = new HashSet<int> {
        0b1111,
        0b0110,
        0b0011,
        0b0010,
        0b1010,
        0b1100,
        0b1110,
        0b1011,
        0b0111
    };

    public static HashSet<int> wallWest = new HashSet<int> {
        0b0100
    };

    public static HashSet<int> wallEast = new HashSet<int> {
        0b0001
    };

    public static HashSet<int> wallSouth = new HashSet<int> {
        0b1000
    };

    public static HashSet<int> wallCornerNorthEastOpen = new HashSet<int> {
        0b11110001,
        0b11100000,
        0b11110000,
        0b11100001,
        0b10100000,
        0b01010001,
        0b11010001,
        0b01100001,
        0b11010000,
        0b01110001,
        0b00010001,
        0b10110001,
        0b10100001,
        0b10010000,
        0b00110001,
        0b10110000,
        0b00100001,
        0b10010001
    };

    public static HashSet<int> wallCornerNorthWestOpen = new HashSet<int> {
        0b11000111,
        0b11000011,
        0b10000011,
        0b10000111,
        0b10000010,
        0b01000101,
        0b11000101,
        0b01000011,
        0b10000101,
        0b01000111,
        0b01000100,
        0b11000110,
        0b11000010,
        0b10000100,
        0b01000110,
        0b10000110,
        0b11000100,
        0b01000010
    };

    public static HashSet<int> wallCornerSouthEastOpen = new HashSet<int> {
    };

    public static HashSet<int> wallCornerSouthWestOpen = new HashSet<int> {

    };

    public static HashSet<int> wallCornerSouthWest = new HashSet<int> {
        0b01000000
    };

    public static HashSet<int> wallCornerSouthEast = new HashSet<int> {
        0b00000001
    };

    public static HashSet<int> wallCornerNorthWest = new HashSet<int> {
        0b00010000,
        0b01010000,
    };

    public static HashSet<int> wallCornerNorthEast = new HashSet<int> {
        0b00000100,
        0b00000101
    };

    public static HashSet<int> wallFull = new HashSet<int> {
        0b1101,
        0b0101,
        0b1101,
        0b1001
    };

    public static HashSet<int> wallFullEightDirections = new HashSet<int> {
        0b00010100,
        0b11100100,
        0b10010011,
        0b01110100,
        0b00010111,
        0b00010110,
        0b00110100,
        0b00010101,
        0b01010100,
        0b00010010,
        0b00100100,
        0b00010011,
        0b01100100,
        0b10010111,
        0b11110100,
        0b10010110,
        0b10110100,
        0b11100101,
        0b11010011,
        0b11110101,
        0b11010111,
        0b11010111,
        0b11110101,
        0b01110101,
        0b01010111,
        0b01100101,
        0b01010011,
        0b01010010,
        0b00100101,
        0b00110101,
        0b01010110,
        0b11010101,
        0b11010100,
        0b10010101
    };

    public static HashSet<int> wallSouthEightDirections = new HashSet<int> {
        0b01000001
    };
}
