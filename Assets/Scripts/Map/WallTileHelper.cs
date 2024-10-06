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
}
