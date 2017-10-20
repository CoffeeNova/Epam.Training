using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epam.Training._01AdvancedCS.TaskLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Epam.Training._01AdvancedCS.TaskLibrary.Types;
using KellermanSoftware.CompareNetObjects;
using Moq;

namespace Epam.Training._01AdvancedCS.TaskLibrary.Tests
{
    [TestClass()]
    public class FileSystemVisitorTests
    {
        public TestContext TestContext { get; set; }

        private static Mock<IFileSystem> _fileSystemMock = new Mock<IFileSystem>();
        private static Mock<IDirectoryInfoFactory> _directoryInfoFactoryMock = new Mock<IDirectoryInfoFactory>();

        private static Mock<DirectoryInfoBase> _rootDirMock = new Mock<DirectoryInfoBase>();
        private static Mock<DirectoryInfoBase> _subRootDir1Mock = new Mock<DirectoryInfoBase>();
        private static Mock<DirectoryInfoBase> _subRootDir2Mock = new Mock<DirectoryInfoBase>();
        private static Mock<DirectoryInfoBase> _subSubRootDir1Mock = new Mock<DirectoryInfoBase>();

        private static Mock<FileInfoBase> _fileInfo1Mock = new Mock<FileInfoBase>();
        private static Mock<FileInfoBase> _fileInfo2Mock = new Mock<FileInfoBase>();
        private static Mock<FileInfoBase> _fileInfo3Mock = new Mock<FileInfoBase>();
        private static Mock<FileInfoBase> _fileInfo4Mock = new Mock<FileInfoBase>();

        private static FileSystemInfoBase[] _fileSystemCollection1;
        private static FileSystemInfoBase[] _fileSystemCollection2;
        private static FileSystemInfoBase[] _fileSystemCollection3;
        private static FileSystemInfoBase[] _fileSystemCollection4;

        private ComparisonConfig _config;
        private const string RootPath = @"D:\\rootDir";

        [ClassInitialize]
        public static void ClassinInitialize(TestContext context)
        {
            #region setup dir and files mocks

            SetupFileInfoBaseStub(_rootDirMock, "rootDir", @"D:\\rootDir");
            SetupFileInfoBaseStub(_subRootDir1Mock, "subRootDir1", @"D:\\rootDir\subRootDir1");
            SetupFileInfoBaseStub(_subRootDir2Mock, "subRootDir2", @"D:\\rootDir\subRootDir2");
            SetupFileInfoBaseStub(_subSubRootDir1Mock, "subSubRootDir1", @"D:\\rootDir\subRootDir1\subSubRootDir1");

            SetupFileInfoBaseStub(_fileInfo1Mock, "file1", @"D:\\rootDir\file1");
            SetupFileInfoBaseStub(_fileInfo2Mock, "file2", @"D:\\rootDir\subRootDir1\file2");
            SetupFileInfoBaseStub(_fileInfo3Mock, "file3", @"D:\\rootDir\subRootDir1\file3");
            SetupFileInfoBaseStub(_fileInfo4Mock, "file4", @"D:\\rootDir\subRootDir1\subSubRootDir1\file4");

            #endregion

            _fileSystemCollection1 = new FileSystemInfoBase[] { _subRootDir1Mock.Object, _subRootDir2Mock.Object, _fileInfo1Mock.Object}; //collection from root dir
            _fileSystemCollection2 = new FileSystemInfoBase[] { _subSubRootDir1Mock.Object, _fileInfo2Mock.Object, _fileInfo3Mock.Object}; //collection from subRootDir1
            _fileSystemCollection3 = new FileSystemInfoBase[] { _fileInfo4Mock.Object}; //collection from subSubRootDir1
            _fileSystemCollection4 = new FileSystemInfoBase[] {}; //collection from subRootDir2

            _rootDirMock.Setup(x => x.GetFileSystemInfos())
                .Returns(_fileSystemCollection1);
            _subRootDir1Mock.Setup(x => x.GetFileSystemInfos())
                .Returns(_fileSystemCollection2);
            _subRootDir2Mock.Setup(x => x.GetFileSystemInfos())
                .Returns(_fileSystemCollection4);
            _subSubRootDir1Mock.Setup(x => x.GetFileSystemInfos())
                .Returns(_fileSystemCollection3);

            _directoryInfoFactoryMock.Setup(x => x.FromDirectoryName(RootPath))
                .Returns(_rootDirMock.Object);

            _fileSystemMock.Setup(x => x.DirectoryInfo)
                .Returns(_directoryInfoFactoryMock.Object);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (TestContext.TestName.Equals(nameof(DirectoryNotFoundException_Exception_Expected)))
                SetupFileInfoBaseStub(_rootDirMock, "rootDir", @"D:\\rootDir", false);
        }

        [TestMethod()]
        public void Should_Search_All_Items()
        {
            //Arrange
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemMock.Object);
            List<FileSystemInfoBase> actual = null;
            fileSystemVisitor.SearchCompletedEvent += delegate(IEnumerable<FileSystemInfoBase> s)
            {
                actual = s.ToList();
            };
            var expected = new List<FileSystemInfoBase>
            {
                _subRootDir1Mock.Object,
                _subRootDir2Mock.Object,
                _subSubRootDir1Mock.Object,
                _fileInfo1Mock.Object,
                _fileInfo2Mock.Object,
                _fileInfo3Mock.Object,
                _fileInfo4Mock.Object
            };
            _config = new ComparisonConfig {IgnoreCollectionOrder = true, };
            var compareLogic = new CompareLogic(_config);

            //act
            fileSystemVisitor.StartSearch(RootPath).Wait();

            //Assert
            var comparisonResult = compareLogic.Compare(expected.Select(x => x.FullName), actual.Select(x => x.FullName));
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), 7);
            Assert.IsTrue(comparisonResult.AreEqual, comparisonResult.DifferencesString);
        }

        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void DirectoryNotFoundException_Exception_Expected()
        {
            //Arrange
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemMock.Object);

            //act
            fileSystemVisitor.StartSearch(RootPath).Wait();
        }

        [TestMethod()]
        public void Should_Return_Items_Where_Names_Includes_1()
        {
            //Arrange
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemMock.Object, x => x.Name.Contains('1'));
            List<FileSystemInfoBase> actual = null;
            fileSystemVisitor.SearchCompletedEvent += delegate (IEnumerable<FileSystemInfoBase> s)
            {
                actual = s.ToList();
            };
            var expected = new List<FileSystemInfoBase>
            {
                _subRootDir1Mock.Object,
                _subSubRootDir1Mock.Object,
                _fileInfo1Mock.Object,
            };
            _config = new ComparisonConfig { IgnoreCollectionOrder = true, };
            var compareLogic = new CompareLogic(_config);

            //act
            fileSystemVisitor.StartSearch(RootPath).Wait();

            //Assert
            var comparisonResult = compareLogic.Compare(expected.Select(x => x.FullName), actual.Select(x => x.FullName));
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), 3);
            Assert.IsTrue(comparisonResult.AreEqual, comparisonResult.DifferencesString);
        }

        [TestMethod()]
        public void Should_Return_Result_Where_Names_Not_Contains_1()
        {
            //Arrange
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemMock.Object);
            List<FileSystemInfoBase> actual = null;
            fileSystemVisitor.SearchCompletedEvent += delegate (IEnumerable<FileSystemInfoBase> s)
            {
                actual = s.ToList();
            };
            fileSystemVisitor.DirectoryFoundEvent += delegate (DirectoryInfoBase dir, ref bool breakSearch, ref bool excludeDir)
            {
                if (dir.Name.Contains('1'))
                    excludeDir = true;
            };

            var expected = new List<FileSystemInfoBase>
            {
                _subRootDir2Mock.Object,
                _fileInfo1Mock.Object,
            };
            _config = new ComparisonConfig { IgnoreCollectionOrder = true, };
            var compareLogic = new CompareLogic(_config);

            //act
            fileSystemVisitor.StartSearch(RootPath).Wait();

            //Assert
            var comparisonResult = compareLogic.Compare(expected.Select(x => x.FullName), actual.Select(x => x.FullName));
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), 2);
            Assert.IsTrue(comparisonResult.AreEqual, comparisonResult.DifferencesString);
        }

        [TestMethod()]
        public void Should_Break_Search_After_Found_File_With_Name_file2()
        {
            //Arrange
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemMock.Object);
            List<FileSystemInfoBase> actual = null;
            fileSystemVisitor.SearchCompletedEvent += delegate (IEnumerable<FileSystemInfoBase> s)
            {
                actual = s.ToList();
            };
            fileSystemVisitor.FileFoundEvent += delegate (FileInfoBase file, ref bool breakSearch, ref bool excludeDir)
            {
                if (file.Name.Equals("file2"))
                    breakSearch = true;
            };

            var expected = new List<FileSystemInfoBase>
            {
                _subRootDir1Mock.Object,
                _subSubRootDir1Mock.Object,
                _fileInfo2Mock.Object,
                _fileInfo4Mock.Object
            };
            _config = new ComparisonConfig { IgnoreCollectionOrder = true, };
            var compareLogic = new CompareLogic(_config);

            //act
            fileSystemVisitor.StartSearch(RootPath).Wait();

            //Assert
            var comparisonResult = compareLogic.Compare(expected.Select(x => x.FullName), actual.Select(x => x.FullName));
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), 4);
            Assert.IsTrue(comparisonResult.AreEqual, comparisonResult.DifferencesString);
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (TestContext.TestName.Equals(nameof(DirectoryNotFoundException_Exception_Expected)))
                SetupFileInfoBaseStub(_rootDirMock, "rootDir", @"D:\\rootDir");
        }

        private static void SetupFileInfoBaseStub<TInfoBase>(Mock<TInfoBase> mock, string name, string fullName, bool exist = true) where TInfoBase : FileSystemInfoBase
        {
            mock.SetupGet(x => x.Exists)
                .Returns(exist);
            mock.SetupGet(x => x.Name)
                .Returns(name);
            mock.SetupGet(x => x.FullName)
                .Returns(fullName);
        }
    }
}