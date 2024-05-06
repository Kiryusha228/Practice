#include <stdio.h>
#include <filesystem>
#include <iostream>
#include "pch.h"
#include "opencv2/opencv.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "string.h"
#define EXPORT extern "C" __declspec(dllexport)

using namespace cv;
using namespace std;


EXPORT void alignAndSaveImages(const unsigned char* imageData1, int imageDataSize1, const unsigned char* imageData2, int imageDataSize2, 
        unsigned char** alignedImageData1, int* alignedImageDataSize1, unsigned char** alignedImageData2, int* alignedImageDataSize2) {
    // �������� �����������
    vector<unsigned char> data1(imageData1, imageData1 + imageDataSize1);
    vector<unsigned char> data2(imageData2, imageData2 + imageDataSize2);

    // ������ ����������� �� �������� ������
    Mat image1 = imdecode(data1, IMREAD_COLOR);
    Mat image2 = imdecode(data2, IMREAD_COLOR);

    // ����� �������� ����� � ���������� �� ������������
    Ptr<Feature2D> detector = ORB::create();
    vector<KeyPoint> keypoints1, keypoints2;
    Mat descriptors1, descriptors2;
    detector->detectAndCompute(image1, Mat(), keypoints1, descriptors1);
    detector->detectAndCompute(image2, Mat(), keypoints2, descriptors2);

    // ������������� �������� �����
    BFMatcher matcher(NORM_HAMMING);
    vector<DMatch> matches;
    matcher.match(descriptors1, descriptors2, matches);

    // ���������� �������������
    double minDist = DBL_MAX;
    for (const auto& match : matches) {
        if (match.distance < minDist) {
            minDist = match.distance;
        }
    }
    vector<DMatch> goodMatches;
    double thresholdDist = 3 * minDist;
    for (const auto& match : matches) {
        if (match.distance <= thresholdDist) {
            goodMatches.push_back(match);
        }
    }

    // ���������� ������� ����������
    vector<Point2f> points1, points2;
    for (const auto& match : goodMatches) {
        points1.push_back(keypoints1[match.queryIdx].pt);
        points2.push_back(keypoints2[match.trainIdx].pt);
    }
    Mat H = findHomography(points1, points2, RANSAC);

    // ���������� ������� ���������� � ����������� 1
    Mat alignedImage1;
    warpPerspective(image1, alignedImage1, H, image1.size());

    // �������������� ����������� � ������� ������
    std::vector<uchar> buffer1, buffer2;
    imencode(".jpg", alignedImage1, buffer1);
    imencode(".jpg", image2, buffer2);

    // ����������� ������ � �������� ������
    *alignedImageDataSize1 = buffer1.size();
    *alignedImageDataSize2 = buffer2.size();
    *alignedImageData1 = new unsigned char[*alignedImageDataSize1];
    *alignedImageData2 = new unsigned char[*alignedImageDataSize2];
    memcpy(*alignedImageData1, buffer1.data(), *alignedImageDataSize1);
    memcpy(*alignedImageData2, buffer2.data(), *alignedImageDataSize2);
}