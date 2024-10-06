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

    public static HashSet<int> wallSouthEightDirections = new HashSet<int> {
        0b01000001
    };

    // TODO: bitmask
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
}
